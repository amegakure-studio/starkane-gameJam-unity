using System;
using System.Collections.Generic;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Character character;
    private bool clicked = false;
    private bool canInteract = true;

    public Character Character { get => character; set => character = value; }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.CHARACTER_MOVE_START, HandleMovementStart);
        EventManager.Instance.Subscribe(GameEvent.CHARACTER_MOVE_END, HandleMovementEnd);

        EventManager.Instance.Subscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
    }

    private void OnDisable()
    {
        EventManager.Instance.Subscribe(GameEvent.CHARACTER_MOVE_START, HandleMovementStart);
        EventManager.Instance.Unsubscribe(GameEvent.CHARACTER_MOVE_END, HandleMovementEnd);
        EventManager.Instance.Unsubscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
    }

    private void HandleMovementStart(Dictionary<string, object> context)
    {
        Character contextCharacter  = (Character) context["Character"];
        
        if(contextCharacter == this.character)
            canInteract = false;
    }

    private void HandleMovementEnd(Dictionary<string, object> context)
    {
        Character contextCharacter  = (Character) context["Character"];
        
        if(contextCharacter == this.character)
        {
            canInteract = true;
            clicked = false;
        }
            
    }

    private void HandleTileSelected(Dictionary<string, object> context)
    {
        try
        {
            if(canInteract)
            {
                Tile tile = (Tile) context["Tile"];            
                character.Move(tile);
            }
            
        }
        catch{}
    }

    private void OnMouseEnter()
    {
        if(!clicked && canInteract)
            CharacterHoverEnter();
    }

    private void OnMouseExit()
    {
        if (!clicked)
            CharacterHoverExit();
    }

    private void OnMouseDown()
    {
        if(canInteract)
        {
            if (!clicked)
            {
                CharacterSelected();
                clicked = true;
            }
            
            else
            {
                CharacterHoverExit();
                clicked = false;
            }
        }
        
    }

    public void CharacterHoverEnter()
    {
        EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_SELECTED, new() { { "Character", character }});
    }

    public void CharacterHoverExit()
    {
        EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_UNSELECTED, new() { { "Character", character }});
    }

    public void CharacterSelected()
    {
        EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_SELECTED, new() { { "Character", character }});
    }
}
