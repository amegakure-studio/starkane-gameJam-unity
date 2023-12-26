using System;
using System.Collections.Generic;
using Amegakure.Starkane;
using Amegakure.Starkane.Context;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.Id;
using Amegakure.Starkane.PubSub;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private CharacterId id;
    private bool clicked = false;

    public CharacterId Id { get => id; set => id = value; }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
    }

    private void HandleTileSelected(Dictionary<string, object> context)
    {
        try
        {
            Tile tile = (Tile) context["Tile"];
            
            GameObject characterGo = id.CharacterGo;

            characterGo.TryGetComponent<WorldCharacterContext>(out WorldCharacterContext characterContext);

            GridMovement movement = characterGo.GetComponent<GridMovement>();
            movement.GoTo(characterContext.Location, characterContext, tile);
            clicked = false;
        }
        catch{}
    }

    private void OnMouseEnter()
    {
        CharacterHoverEnter();
    }

    private void OnMouseExit()
    {
        if (!clicked)
            CharacterHoverExit();
    }

    private void OnMouseDown()
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

    public void CharacterHoverEnter()
    {
        EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_SELECTED, new() { { "CharacterId", id }, { "CharacterGo", gameObject } });
    }

    public void CharacterHoverExit()
    {
        EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_UNSELECTED, new() { { "CharacterId", id }, { "CharacterGo", gameObject } });
    }

    public void CharacterSelected()
    {
        EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_SELECTED, new() { { "CharacterId", id }, { "CharacterGo", gameObject } });
    }
}
