using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using bottlenoselabs.C2CS.Runtime;
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
            Debug.Log(username + " : " + password);
            
            var playerHash = new Hash128();
            playerHash.Append(username);
            playerHash.Append(password);
            
            string playerId = playerHash.ToString();
            
            Debug.Log("HASH: " + playerId);

            Session session = CreateSessionObj();
            Player sessionPlayer = FindSessionPlayer(username, playerId, session.gameObject);
            if (sessionPlayer != null)
            {
                session.Player = sessionPlayer;
            }
            else
            {
                Player player = CreatePlayer(username, playerId, session.gameObject);
                session.Player = player;
                
                try
                {
                    CallCreatePlayerTx(playerId);
                }
                catch (Exception e)
                {
                    Debug.LogError("Can't log-in: " + e);
                }
            }
            
            StartCoroutine(nameof(LoadAsyncScene));
        }
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private Player CreatePlayer(string sessionPlayername, string sessionPlayerStringId, GameObject sessionGo)
    {
        var player_id = dojo.felt_from_hex_be(new CString(sessionPlayerStringId)).ok;
        var hexString = BitConverter.ToString(player_id.data.ToArray()).Replace("-", "").ToLower();
        BigInteger intID = BigInteger.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);

        Debug.Log("!!HASH BIN: " + intID);

        Player player = sessionGo.AddComponent<Player>();
        player.Id = intID;
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
        var player_id = dojo.felt_from_hex_be(new CString(sessionPlayerStringId)).ok;

        var hexString = BitConverter.ToString(player_id.data.ToArray()).Replace("-", "").ToLower();
        BigInteger intID = BigInteger.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
        Debug.Log("Bing Int ID: " + intID);

        GameObject[] entities = worldManager.Entities();

        foreach (GameObject go in entities)
        {
            CharacterPlayerProgress characterPlayerProgress = go.GetComponent<CharacterPlayerProgress>();
            if (characterPlayerProgress != null)
            {
                bool res = characterPlayerProgress.getPlayerID().Equals(intID);
                if (res)
                {
                    Player player = sessionGo.AddComponent<Player>();
                    player.Id = intID;
                    player.SetDojoId(characterPlayerProgress.Owner);
                    player.PlayerName = sessionPlayername;
                    player.DefaultCharacter = characterPlayerProgress.GetCharacterType();
                    Debug.Log("Session player Found!!");
                    return player;
                }
            }
        }

        return null;
    }

    private void CallCreatePlayerTx(string playerId)
    {
        DojoTxConfig dojoTxConfig = GameObject.FindAnyObjectByType<DojoTxConfig>();
        var provider = new JsonRpcClient(dojoTxConfig.RpcUrl);
        var account = new Account(provider, dojoTxConfig.GetKatanaPrivateKey(), dojoTxConfig.KatanaAccounAddress);
        
        var character_id = dojo.felt_from_hex_be(new CString("0x03")).ok;
        var player_id = dojo.felt_from_hex_be(new CString(playerId)).ok;

        dojo.Call call = new dojo.Call()
        {
            calldata = new dojo.FieldElement[]
            {
                        character_id,
                        player_id,
                        dojo.felt_from_hex_be(new CString("0x01")).ok
            },
            to = dojoTxConfig.CharacterSystemActionAddress,
            selector = "mint"
        };

        try
        {
            account.ExecuteRaw(new[] { call });
        }
        catch (Exception e)
        {
            Debug.Log("Cannot create new player");
            Debug.LogError(e);
        }

        Debug.Log("Success!!!");

    }
}
