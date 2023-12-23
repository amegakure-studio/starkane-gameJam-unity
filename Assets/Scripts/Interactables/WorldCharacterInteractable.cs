using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterInteractable : InteractableBehaviour
{
    CharacterMovement characterMovement;
    Interactable interactable;

    private void Start() 
    { 
        characterMovement = GetComponent<CharacterMovement>(); 
        interactable = GetComponent<Interactable>();
    }

    private void OnEnable() { EventManager.Instance.Subscribe(GameEvent.TILE_SELECTED, HandleTileSelected); }

    private void OnDisable() { EventManager.Instance.Unsubscribe(GameEvent.TILE_SELECTED, HandleTileSelected); }

    private void HandleTileSelected(Dictionary<string, object> context)
    {
        if (interactable.IsInteracting())
        {
            try
            {
                Tile target = (Tile)context["Tile"];
                CharacterMovement characterMovement = GetComponent<CharacterMovement>();

                characterMovement.GoTo(target);
                interactable.EndInteraction();

            }
            catch (Exception e) { Debug.LogError(e); }
        }
    }

    public override bool CanInteract() {  return !characterMovement.Moving; }

    public override void Interact() { characterMovement.FindPaths(characterMovement.Location); }

    public override void Uninteract() { characterMovement.ClearFrontier(); }
}
