using System;
using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
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

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.INPUT_ENEMY_COMBAT_INTERACTION, HandleEnemyInteraction);
            EventManager.Instance.Subscribe(GameEvent.CUTSCENE_COMBAT_ACCEPT, HandleAcceptInteraction);
            EventManager.Instance.Subscribe(GameEvent.CUTSCENE_COMBAT_CANCEL, HandleCancelInteraction);
        }
        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_ENEMY_COMBAT_INTERACTION, HandleEnemyInteraction);
            EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_COMBAT_ACCEPT, HandleAcceptInteraction);
            EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_COMBAT_CANCEL, HandleCancelInteraction);
        }

        private void HandleAcceptInteraction(Dictionary<string, object> dictionary)
        {
            Character character = GameObject.FindObjectOfType<Character>();

            foreach (var output in battleDirector.playableAsset.outputs)
            {
                // Get the binding for each output
                var binding = battleDirector.GetGenericBinding(output.sourceObject);

                // Output information about the binding
                Debug.Log($"Binding Name: {output.streamName}, Target Object: {binding}");
    
                if(output.streamName == "Animation Track")
                {
                    Animator animator = character.gameObject.GetComponent<Animator>();
                    battleDirector.SetGenericBinding(output.sourceObject, animator);
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
                Debug.Log($"Binding Name: {output.streamName}, Target Object: {binding}");
    
                if(output.streamName == "Animation Track (1)")
                {
                    Animator animator = character.gameObject.GetComponent<Animator>();
                    dismissDirector.SetGenericBinding(output.sourceObject, animator);
                }
            }

            CancelBattle();
        }

        private void HandleEnemyInteraction(Dictionary<string, object> dictionary)
        {
            Character character = GameObject.FindObjectOfType<Character>();

            foreach (var output in encounterDirector.playableAsset.outputs)
            {
                // Get the binding for each output
                var binding = encounterDirector.GetGenericBinding(output.sourceObject);

                // Output information about the binding
                Debug.Log($"Binding Name: {output.streamName}, Target Object: {binding}");
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
    }

}
