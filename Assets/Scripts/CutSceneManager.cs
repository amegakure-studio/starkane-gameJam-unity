using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] PlayableDirector encounterDirector;
    [SerializeField] PlayableDirector battleDirector;
    [SerializeField] PlayableDirector dismissDirector;
    private bool m_HasEncounterTrigger = false;
    

    private void OnEnable() { EventManager.Instance.Subscribe(GameEvent.ENCOUNTER_INTERACTION, HandleEncounter); }

    private void OnDisable() { EventManager.Instance.Unsubscribe(GameEvent.ENCOUNTER_INTERACTION, HandleEncounter); }

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

    public void EnableUIActions()
    {
        EventManager.Instance.Publish(GameEvent.SHOW_DIALOG_ENCOUNTER, null);
        m_HasEncounterTrigger = false;
    }
}
