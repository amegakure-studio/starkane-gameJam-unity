using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using Dojo;
using Dojo.Starknet;
using dojo_bindings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoginController : MonoBehaviour
{
    private Button loginBtn;
    private TextField usernameTxtField;
    private TextField passwordTxtField;
    [SerializeField] WorldManager worldManager;

    private void Start()
    {
        VisualElement root = GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement;

        usernameTxtField = root.Q<TextField>("Username");
        passwordTxtField = root.Q<TextField>("Password");

        loginBtn = root.Q<Button>("LoginBtn");
        loginBtn.clicked += LoginBtn_clicked;
    }

    private void LoginBtn_clicked()
    {
        string username = usernameTxtField.text;
        string password = passwordTxtField.text;

        if (!string.IsNullOrWhiteSpace(username) &&
           !string.IsNullOrWhiteSpace(password))
        {
            var playerHash = new Hash128();
            playerHash.Append(username);
            playerHash.Append(password);

            var playerFieldElement = new FieldElement(playerHash.ToString()).Inner();
            string playerId = "0x" + BitConverter.ToString(playerFieldElement.data.ToArray()).Replace("-", "").ToLower();
            
            //Debug.Log("playerId: " + playerId);

            Session session = CreateSessionObj();
            Player sessionPlayer = FindSessionPlayer(username, playerId, session.gameObject);
            
            if (sessionPlayer != null)
            {
                session.Player = sessionPlayer;
                StartCoroutine(nameof(LoadAsyncScene));
            }
            else
            {
                Player player = CreatePlayer(username, playerId, session.gameObject);
                session.Player = player;

                try
                {
                    StartCoroutine(nameof(MintCoroutine), playerHash.ToString());
                }
                catch (Exception e)
                {
                    Debug.LogError("Can't log-in: " + e);
                }
            }

        }
    }

    IEnumerator MintCoroutine(string playerId)
    {
        CallCreatePlayerTx(playerId, "0x03");
        Debug.Log("Mint");

        yield return new WaitForSeconds(2);
        Debug.Log("2-Mint");
        try
        {
            CallCreatePlayerTx(playerId, "0x01");
            StartCoroutine(nameof(LoadAsyncScene));
        }
        catch{}
        
        yield return null;
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        asyncLoad.allowSceneActivation = false;

        EventManager.Instance.Publish(GameEvent.GAME_LOADING_START, new Dictionary<string, object>());

        while (!asyncLoad.isDone)
        {
            //Debug.Log("Still here: " + asyncLoad.progress);
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }

    private Player CreatePlayer(string sessionPlayername, string sessionPlayerStringId, GameObject sessionGo)
    {
        Player player = sessionGo.AddComponent<Player>();

        player.HexID = sessionPlayerStringId;
        player.PlayerName = sessionPlayername;
        player.DefaultCharacter = CharacterType.Warrior;
        
        return player;
    }

    private Session CreateSessionObj()
    {
        GameObject sessionGo = Instantiate(new GameObject());
        sessionGo.name = "Session";

        Session session = sessionGo.AddComponent<Session>();

        DontDestroyOnLoad(sessionGo);
        // DontDestroyOnLoad(session.Player);

        return session;
    }

    private Player FindSessionPlayer(string sessionPlayername, string sessionPlayerStringId, GameObject sessionGo)
    {
        List<CharacterPlayerProgress> characterPlayerProgresses = GetCharacterPlayerProgressesFromPlayerId(sessionPlayerStringId);
        
        if (characterPlayerProgresses.Count > 0)
        {
            Debug.Log("Session player Found!!");

            CharacterPlayerProgress defaultCharacterPlayerProgress = GetDefaultCharacter(characterPlayerProgresses, CharacterType.Warrior);

            if (defaultCharacterPlayerProgress == null)
                defaultCharacterPlayerProgress = characterPlayerProgresses[0];

            Player player = sessionGo.AddComponent<Player>();
            
            // TODO: Must be the string
            player.HexID = sessionPlayerStringId;
            player.SetDojoId(defaultCharacterPlayerProgress.Owner);
            player.PlayerName = sessionPlayername;
            player.DefaultCharacter = defaultCharacterPlayerProgress.GetCharacterType(); 
            
            return player;
        }

        return null;
    }

    private CharacterPlayerProgress GetDefaultCharacter(List<CharacterPlayerProgress> characterPlayerProgresses, CharacterType warrior)
    {
        return characterPlayerProgresses.Find(cp => cp.GetCharacterType() == warrior);
    }

    private List<CharacterPlayerProgress> GetCharacterPlayerProgressesFromPlayerId(string playerID)
    {
        List<CharacterPlayerProgress> players = new();
        
        GameObject[] entities = worldManager.Entities();
        foreach (GameObject go in entities)
        {
            CharacterPlayerProgress characterPlayerProgress = go.GetComponent<CharacterPlayerProgress>();
            if (characterPlayerProgress != null)
            {
                bool res = characterPlayerProgress.owner.Hex().Equals(playerID);

                if (res)
                    players.Add(characterPlayerProgress);
            }
        }

        return players;
    }

    private async void CallCreatePlayerTx(string playerId, string characterId)
    {
        DojoTxConfig dojoTxConfig = GameObject.FindAnyObjectByType<DojoTxConfig>();
        var provider = new JsonRpcClient(dojoTxConfig.RpcUrl);
        var account = new Account(provider, dojoTxConfig.GetKatanaPrivateKey(), new FieldElement(dojoTxConfig.KatanaAccounAddress));

        var character_id = new FieldElement(characterId).Inner();
        var player_id = new FieldElement(playerId).Inner();

        dojo.Call call = new dojo.Call()
        {
            calldata = new dojo.FieldElement[]
            {
                        character_id,
                        player_id,
                        new FieldElement("0x01").Inner()
            },
            to = dojoTxConfig.CharacterSystemActionAddress,
            selector = "mint"
        };

        try
        {
            await account.ExecuteRaw(new[] { call });
        }
        catch (Exception e)
        {
            Debug.Log("Cannot create new player");
            Debug.LogError(e);
        }
    }
}
