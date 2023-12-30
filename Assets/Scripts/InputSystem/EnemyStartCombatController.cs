using System;
using System.Collections.Generic;
using Amegakure.Starkane.PubSub;
using Amegakure.Starkane.UI.NonDiegetic;
using UnityEngine;

public class EnemyStartCombatController : MonoBehaviour
{
    [SerializeField] Dialog dialog;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.CUTSCENE_ENCOUNTER_ENEMY_END, HandleEncounter);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_ENCOUNTER_ENEMY_END, HandleEncounter);
    }

    private void HandleEncounter(Dictionary<string, object> dictionary)
    {
        dialog.Show();
    }


    private void OnMouseDown()
    {
        Dictionary<string, object> context = new(){ { "Enemy", this } };

        EventManager.Instance.Publish(GameEvent.INPUT_COMBAT_CHALLENGE, context);
    }
}
