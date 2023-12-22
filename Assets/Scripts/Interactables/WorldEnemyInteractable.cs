using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldEnemyInteractable : InteractableBehaviour
{
    Dialog dialog;

    private void Start()
    {
        dialog = GameObject.FindFirstObjectByType<Dialog>();
    }

    private void OnEnable() { EventManager.Instance.Subscribe(GameEvent.SHOW_DIALOG_ENCOUNTER, HandleShowDialog); }

    private void OnDisable() { EventManager.Instance.Unsubscribe(GameEvent.SHOW_DIALOG_ENCOUNTER, HandleShowDialog); }

    private void HandleShowDialog(Dictionary<string, object> dictionary)
    {
        dialog.Show();  
    }

    public override bool CanInteract()
    {
        return true;
    }

    public override void Interact()
    {
        EventManager.Instance.Publish(GameEvent.ENCOUNTER_INTERACTION, null);    
    }

    public override void Uninteract()
    {
        //dialog.Hide();
    }
}
