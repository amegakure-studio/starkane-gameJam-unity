using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldEnemyInteractable : InteractableBehaviour
{
    Dialog dialog;
    Interactable interactable;

    private void Start()
    {
        dialog = GameObject.FindFirstObjectByType<Dialog>();
        interactable = GetComponent<Interactable>();
    }

    private void OnEnable() { EventManager.Instance.Subscribe(GameEvent.CUTSCENE_ENCOUNTER_ENEMY_END, HandleShowDialog); }

    private void OnDisable() { EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_ENCOUNTER_ENEMY_END, HandleShowDialog); }

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
        EventManager.Instance.Publish(GameEvent.INTERACT_ENCOUNTER_ENEMY, new Dictionary<string, object>());
        interactable.EndInteraction();    
    }

    public override void Uninteract()
    {
        //dialog.Hide();
    }
}
