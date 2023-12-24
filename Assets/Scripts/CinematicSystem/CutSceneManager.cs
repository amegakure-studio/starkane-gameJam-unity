using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Amegakure.Starkane.CinematicSystem
{
    public class CutSceneManager : MonoBehaviour
    {
        [SerializeField] PlayableDirector encounterDirector;
        [SerializeField] PlayableDirector battleDirector;
        [SerializeField] PlayableDirector dismissDirector;
        private bool m_HasEncounterTrigger = false;
    
        private void CancelBattle(Dictionary<string, object> dictionary)
        {
            dismissDirector.Stop();
            dismissDirector.time = 0;
            dismissDirector.Play();
        }

        private void HandleEncounter(Dictionary<string, object> context)
        {
            if(!m_HasEncounterTrigger)
            {
                encounterDirector.Stop();
                encounterDirector.time = 0;
                encounterDirector.Play();
                m_HasEncounterTrigger = true;
            }
        }

        private void HandleBattle(Dictionary<string, object> dictionary)
        {
            battleDirector.Stop();
            battleDirector.time = 0;
            battleDirector.Play();
        }

        public void EnableUIActions()
        {
            m_HasEncounterTrigger = false;
        }

        public void battleStart()
        {
            SceneManager.LoadScene("Combat");
        }
    }
}
