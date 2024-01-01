using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.InputSystem
{
    public class TileController : MonoBehaviour
    {
        private Tile tile;
        private TileRenderer tileRenderer;
        private Character characterSelected;
        private Combat combat;


        private void Awake()
        {
            tile = GetComponent<Tile>();
        }

        private void Start()
        {
            tileRenderer = GameObject.FindObjectOfType<TileRenderer>();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_SELECTED, handleCharacterSelected);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_SELECTED, handleCharacterSelected);
        }

        private void handleCharacterSelected(Dictionary<string, object> dictionary)
        {
            try
            {
                characterSelected = (Character)dictionary["Character"];

            } catch{}
        }

        private void OnMouseOver()
        {
            if (tile && tile.InFrontier && tileRenderer)
                tileRenderer.SetHighlightTiles(new() { tile });
        }

        private void OnMouseExit()
        {
            if (tileRenderer)
            {
                if (tile && tile.InFrontier)
                    tileRenderer.SetActiveTiles(new() { tile });
                else
                    tileRenderer.ClearColor(new() { tile });
            }       
        }

        private void OnMouseDown()
        {
            if (tile && tile.InFrontier)
            {
                if (tile.IsMovementTile)
                {
                    if(combat == null)
                        combat = GameObject.FindAnyObjectByType<Combat>();

                    Player player = combat.GetPlayerByID(characterSelected.GetPlayerId());

                    Debug.Log("Character selected when selecting tile: " + characterSelected.CharacterName);
                    if (combat.CanMove(characterSelected, player))
                    {
                        Dictionary<string, object> context = new(){ { "Tile", tile } };
                        EventManager.Instance.Publish(GameEvent.TILE_SELECTED, context);
                    }
                }
                
                else
                {
                    Dictionary<string, object> context = new(){ { "Tile", tile } };

                    EventManager.Instance.Publish(GameEvent.TILE_SELECTED, context);
                }
                
            }
        }
    }
}
