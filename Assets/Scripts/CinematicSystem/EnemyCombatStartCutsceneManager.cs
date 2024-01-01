using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using bottlenoselabs.C2CS.Runtime;
using Dojo;
using Dojo.Starknet;
using dojo_bindings;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Amegakure.Starkane.CinematicSystem
{
    public class EnemyCombatStartCutsceneManager : MonoBehaviour
    {
        [SerializeField] PlayableDirector encounterDirector;
        [SerializeField] PlayableDirector battleDirector;
        [SerializeField] PlayableDirector dismissDirector;
        [SerializeField] Vector3 initPos;
        [SerializeField] Transform enemyTransform;

        private GameObject playerGo;
        private Vector3 playerOriginalPos;
        private bool m_HasEncounterTrigger = false;

        private WorldManager worldManager;

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.INPUT_COMBAT_CHALLENGE, HandleCombatChallenge);
            EventManager.Instance.Subscribe(GameEvent.INPUT_COMBAT_ACCEPT, HandleAcceptInteraction);
            EventManager.Instance.Subscribe(GameEvent.INPUT_COMBAT_CANCEL, HandleCancelInteraction);
        }
        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_COMBAT_CHALLENGE, HandleCombatChallenge);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_COMBAT_ACCEPT, HandleAcceptInteraction);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_COMBAT_CANCEL, HandleCancelInteraction);
        }

        private void HandleAcceptInteraction(Dictionary<string, object> dictionary)
        {
            Character character = GameObject.FindObjectOfType<Character>();

            foreach (var output in battleDirector.playableAsset.outputs)
            {
                // Get the binding for each output
                var binding = battleDirector.GetGenericBinding(output.sourceObject);

                // Output information about the binding
                // Debug.Log($"Binding Name: {output.streamName}, Target Object: {binding}");
    
                if(output.streamName == "Animation Track")
                {
                    Animator animator = character.gameObject.GetComponent<Animator>();
                    battleDirector.SetGenericBinding(output.sourceObject, animator);
                    
                    Player player  = GameObject.FindObjectOfType<Session>().Player;
                    BigInteger playerMatchId = player.Id;
                    int playerCharacterId = (int)player.DefaultCharacter;
                    
                    var playerHash = new Hash128();
                    playerHash.Append("enemy");
                    playerHash.Append("enemy");
                    string adversaryId = playerHash.ToString();
                    
                    // Debug.Log("Enemy hash: "+ adversaryId);
                    int adversaryCharacterId = 4;

                    this.LoadOrCreateMatch(playerMatchId, playerCharacterId,
                                            adversaryId, adversaryCharacterId);
                }
            }

            HandleBattle();
        }

        private void HandleCancelInteraction(Dictionary<string, object> dictionary)
        {
             Character character = GameObject.FindObjectOfType<Character>();

            foreach (var output in dismissDirector.playableAsset.outputs)
            {
                // Get the binding for each output
                var binding = dismissDirector.GetGenericBinding(output.sourceObject);

                // Output information about the binding
                // Debug.Log($"Binding Name: {output.streamName}, Target Object: {binding}");
    
                if(output.streamName == "Animation Track (1)")
                {
                    Animator animator = character.gameObject.GetComponent<Animator>();
                    dismissDirector.SetGenericBinding(output.sourceObject, animator);
                }
            }

            CancelBattle();
        }

        private void HandleCombatChallenge(Dictionary<string, object> dictionary)
        {
            Character character = GameObject.FindObjectOfType<Character>();

            foreach (var output in encounterDirector.playableAsset.outputs)
            {
                // Get the binding for each output
                var binding = encounterDirector.GetGenericBinding(output.sourceObject);

                // Output information about the binding
                // Debug.Log($"Binding Name: {output.streamName}, Target Object: {binding}");
                if(output.streamName == "Activation Track")
                {
                    playerGo = character.gameObject;
                    playerOriginalPos = playerGo.transform.position;

                    playerGo.transform.position = initPos;
                    playerGo.transform.rotation = GetAngleBetweenMeAndEnemy(playerGo.transform, enemyTransform);
                    encounterDirector.SetGenericBinding(output.sourceObject, playerGo);
                }
                
                if(output.streamName == "Animation Track")
                {
                    Animator animator = character.gameObject.GetComponent<Animator>();
                    encounterDirector.SetGenericBinding(output.sourceObject, animator);
                }
            }

            HandleEncounter();
        }

    
        private void CancelBattle()
        {
            dismissDirector.Stop();
            dismissDirector.time = 0;
            dismissDirector.Play();
        }

        private void HandleEncounter()
        {
            if(!m_HasEncounterTrigger)
            {
                encounterDirector.Stop();
                encounterDirector.time = 0;
                encounterDirector.Play();
                m_HasEncounterTrigger = true;
            }
        }

        private void HandleBattle()
        {
            battleDirector.Stop();
            battleDirector.time = 0;
            battleDirector.Play();
        }

        public void ResetPlayerPosition()
        {
            playerGo.transform.position = playerOriginalPos;
        }

        public void EnableUIActions()
        {
            Dictionary<string, object> context = new(){ { "CutSceneManager", this } };

            EventManager.Instance.Publish(GameEvent.CUTSCENE_ENCOUNTER_ENEMY_END, context);

            m_HasEncounterTrigger = false;
        }

        public void battleStart()
        {
            StartCoroutine(nameof(LoadAsyncScene));
        }

        IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3);
        asyncLoad.allowSceneActivation = false;

        EventManager.Instance.Publish(GameEvent.GAME_LOADING_START, new Dictionary<string, object>());

        while (!asyncLoad.isDone)
        {
            Debug.Log("Still here: " + asyncLoad.progress);
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }

        private Quaternion GetAngleBetweenMeAndEnemy(Transform playerTransform, Transform enemyTransform)
        {
            Vector3 targetDir =  enemyTransform.position - playerTransform.position;
            Quaternion lookDir = Quaternion.LookRotation(targetDir);
            return lookDir;
        }


        private List<BigInteger> FindMatchPlayers(int match_id)
        {
            List<BigInteger> matchPlayers = new();

            GameObject[] entities = worldManager.Entities();

            foreach (GameObject go in entities)
            {
                try
                {
                    MatchPlayer matchPlayer = go.GetComponent<MatchPlayer>();
                    if (matchPlayer != null)
                    {
                        if ((int) matchPlayer.Match_id == match_id)
                            matchPlayers.Add(matchPlayer.PlayerId);
                    }
                }
                catch { }
            }

            return matchPlayers;
        }


        private MatchState GetMatchState(BigInteger playerId, string adversaryId)
        {
            if(worldManager == null)
                worldManager = GameObject.FindAnyObjectByType<WorldManager>();

            GameObject[] entities = worldManager.Entities();

            foreach(GameObject go in entities)
            {
                try
                {
                    MatchState matchState = go.GetComponent<MatchState>();
                    var adversaryDojoId = dojo.felt_from_hex_be(new CString(adversaryId)).ok;
                    var hexString = BitConverter.ToString(adversaryDojoId.data.ToArray()).Replace("-", "").ToLower();
                    BigInteger adversaryMatchId = BigInteger.Parse( hexString, System.Globalization.NumberStyles.AllowHexSpecifier );

                    if(matchState != null)
                    {
                        List<BigInteger> playerIdInMatch = FindMatchPlayers((int)matchState.Id);
                        
                        bool isPlayerAdversaryMatch = playerIdInMatch.Contains(playerId)
                        && playerIdInMatch.Contains(adversaryMatchId);
                        
                        if (matchState.WinnerId.Equals(0) && isPlayerAdversaryMatch)
                        {
                            return matchState;
                        }
                    }
                }
                catch{}
            }

            return null;
        }
        
        private void CallCreateMatchTX(BigInteger playerId, int playerCharacterId,
                                    string adversayId, int adversayCharacterId)
        {
            DojoTxConfig dojoTxConfig = GameObject.FindAnyObjectByType<DojoTxConfig>();
            var provider = new JsonRpcClient(dojoTxConfig.RpcUrl);
            var account = new Account(provider, dojoTxConfig.GetKatanaPrivateKey(), dojoTxConfig.KatanaAccounAddress);
            
            string playerIdString =  playerId.ToString("X");
            var player_id = dojo.felt_from_hex_be(new CString(playerIdString)).ok;

            string characterIdString =  playerCharacterId.ToString("X");
            var character_id = dojo.felt_from_hex_be(new CString(characterIdString)).ok;
            
            var adversary_id = dojo.felt_from_hex_be(new CString(adversayId)).ok; 

            string adversaryCharacterIdString =  adversayCharacterId.ToString("X");
            var adversary_character_id = dojo.felt_from_hex_be(new CString(adversaryCharacterIdString)).ok;

            
            dojo.Call call = new dojo.Call()
            {
                // calldata = new[]
                // {
                //    dojo.felt_from_hex_be(new CString("0x03")).ok, player_id, character_id,
                //    player_id, dojo.felt_from_hex_be(new CString("0x03")).ok, 
                //    adversary_id, adversary_character_id
                // },
                calldata = new[]
                {
                   dojo.felt_from_hex_be(new CString("0x04")).ok,
                   player_id, character_id,
                   player_id, dojo.felt_from_hex_be(new CString("0x01")).ok, 
                   adversary_id, adversary_character_id,
                   adversary_id, dojo.felt_from_hex_be(new CString("0x06")).ok
                },
                to = dojoTxConfig.MatchSystemActionAddress,
                selector = "init"
            };
    
            account.ExecuteRaw(new[] { call });
        }

        public void LoadOrCreateMatch(BigInteger playerId, int playerCharacterId,
                                    string adversayId, int adversayCharacterId )
        {
            MatchState matchState = this.GetMatchState(playerId, adversayId);
            if(matchState == null)
            {
                CallCreateMatchTX(playerId, playerCharacterId, adversayId, adversayCharacterId);
            }    
        }

        public void MintEnemyCharacters()
        {

        }
    }
    

}
