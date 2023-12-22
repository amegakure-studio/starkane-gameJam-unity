using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] PlayableDirector encounterDirector;
    [SerializeField] PlayableDirector battleDirector;
    [SerializeField] PlayableDirector dismissDirector;
    private bool m_HasEncounterTrigger = false;
    

    private void OnEnable() 
    {
        EventManager.Instance.Subscribe(GameEvent.ENCOUNTER_INTERACTION, HandleEncounter);
        EventManager.Instance.Subscribe(GameEvent.BATTLE_INTERACTION, HandleBattle);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.ENCOUNTER_INTERACTION, HandleEncounter);
        EventManager.Instance.Subscribe(GameEvent.BATTLE_INTERACTION, HandleBattle);
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
        EventManager.Instance.Publish(GameEvent.SHOW_DIALOG_ENCOUNTER, null);
        m_HasEncounterTrigger = false;
    }

    public void battleStart()
    {
        SceneManager.LoadScene("Combat");
    }
}
