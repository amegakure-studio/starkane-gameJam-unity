using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.InputSystem
{
    public class CombatCharacterController : MonoBehaviour
    {
        private Player player;
        private Character character;
        private Combat combat;
        private Character characterSelected;

        private bool clicked = false;
        private bool canInteract = true;


        public Player Player { get => player; set => player = value; }
        public Character Character { get => character; set => character = value; }
        public Combat Combat { get => combat; set => combat = value; }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.CHARACTER_MOVE_START, HandleMovementStart);
            EventManager.Instance.Subscribe(GameEvent.CHARACTER_MOVE_END, HandleMovementEnd);
            EventManager.Instance.Subscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_SELECTED, handleCharacterSelected);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.CHARACTER_MOVE_START, HandleMovementStart);
            EventManager.Instance.Unsubscribe(GameEvent.CHARACTER_MOVE_END, HandleMovementEnd);
            EventManager.Instance.Unsubscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_SELECTED, handleCharacterSelected);
        }

        private void handleCharacterSelected(Dictionary<string, object> dictionary)
        {
            try
            {
                characterSelected = (Character)dictionary["Character"];

            }
            catch { }
        }

        private void HandleMovementStart(Dictionary<string, object> context)
        {
            Character contextCharacter = (Character)context["Character"];

            if (contextCharacter == this.character)
                canInteract = false;
        }

        private void HandleMovementEnd(Dictionary<string, object> context)
        {
            Character contextCharacter = (Character)context["Character"];

            if (contextCharacter == this.character)
            {
                canInteract = true;
                clicked = false;
            }
        }

        private void HandleTileSelected(Dictionary<string, object> context)
        {
            try
            {
                if (canInteract && combat.CanMove(character, player))
                {
                    if (characterSelected.GetInstanceID() == character.GetInstanceID())
                    {
                        Tile tile = (Tile)context["Tile"];
                        if (tile.IsMovementTile)
                            combat.Move(character, player, tile);
                    }
                }
            }
            catch { }
        }

        private void OnMouseEnter()
        {
            if (!clicked && canInteract && combat.CanMove(character, player))
                CharacterHoverEnter();
        }

        private void OnMouseExit()
        {
            if (!clicked && combat.CanMove(character, player))
                CharacterHoverExit();
        }

        private void OnMouseDown()
        {
            if (canInteract && combat.CanMove(character, player))
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
            EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_HOVER, new() { { "Character", character } });
        }

        public void CharacterHoverExit()
        {
            EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_UNHOVER, new() { { "Character", character } });
        }

        public void CharacterSelected()
        {
            EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_SELECTED, new() { { "Character", character } });
        }
    }
}
