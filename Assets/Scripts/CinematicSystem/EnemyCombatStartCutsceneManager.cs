using System;
using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using bottlenoselabs.C2CS.Runtime;
using Dojo;
using Dojo.Starknet;
using dojo_bindings;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

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
                    int playerMatchId = player.Id;
                    int playerCharacterId = (int)player.DefaultCharacter;
                    
                    int adversaryMatchId = 2;
                    int adversaryCharacterId = 4;

                    this.LoadOrCreateMatch(playerMatchId, playerCharacterId,
                                            adversaryMatchId, adversaryCharacterId);
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
            SceneManager.LoadScene("Cavern-combat");
        }

        private Quaternion GetAngleBetweenMeAndEnemy(Transform playerTransform, Transform enemyTransform)
        {
            Vector3 targetDir =  enemyTransform.position - playerTransform.position;
            Quaternion lookDir = Quaternion.LookRotation(targetDir);
            return lookDir;
        }


        private List<int> FindMatchPlayers(int match_id)
        {
            List<int> matchPlayers = new();

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


        private MatchState GetMatchState(int playerId, int adversaryId)
        {
            if(worldManager == null)
                worldManager = GameObject.FindAnyObjectByType<WorldManager>();

            GameObject[] entities = worldManager.Entities();

            foreach(GameObject go in entities)
            {
                try
                {
                    MatchState matchState = go.GetComponent<MatchState>();
                    
                    if(matchState != null)
                    {
                        List<int> playerIdInMatch = FindMatchPlayers((int)matchState.Id);
                        bool isPlayerAdversaryMatch = playerIdInMatch.Contains(playerId) && playerIdInMatch.Contains(adversaryId);
                        if (matchState.WinnerId == 0 && isPlayerAdversaryMatch)
                        {
                            return matchState;
                        }
                    }
                }
                catch{}
            }

            return null;
        }
        
        private void CallCreateMatchTX(int playerId, int playerCharacterId,
                                    int adversayId, int adversayCharacterId)
        {
            string rpcUrl = "http://localhost:5050";

            var provider = new JsonRpcClient(rpcUrl);
            var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
            string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

            var account = new Account(provider, signer, playerAddress);
            string actionsAddress = "0x2d4efd349d469a05680cb7f1186024b8d95c33bd11563de07fe687ddcbfa483";
            
            string playerIdString =  playerId.ToString("X");
            var player_id = dojo.felt_from_hex_be(new CString(playerIdString)).ok;

            string characterIdString =  playerCharacterId.ToString("X");
            var character_id = dojo.felt_from_hex_be(new CString(characterIdString)).ok;
            

            string adversaryIdString =  adversayId.ToString("X");
            var adversary_id = dojo.felt_from_hex_be(new CString(adversaryIdString)).ok; 

            string adversaryCharacterIdString =  adversayCharacterId.ToString("X");
            var adversary_character_id = dojo.felt_from_hex_be(new CString(adversaryCharacterIdString)).ok;

            
            dojo.Call call = new dojo.Call()
            {
                calldata = new[]
                {
                   dojo.felt_from_hex_be(new CString("0x02")).ok, player_id, character_id,
                   adversary_id, adversary_character_id
                },
                to = actionsAddress,
                selector = "init"
            };
    
            account.ExecuteRaw(new[] { call });
            
        }

        public void LoadOrCreateMatch(int playerId, int playerCharacterId,
                                    int adversayId, int adversayCharacterId )
        {
            MatchState matchState = this.GetMatchState(playerId, adversayId);
            if(matchState == null)
            {
                CallCreateMatchTX(playerId, playerCharacterId, adversayId, adversayCharacterId);
            }    
        }
    }

}
