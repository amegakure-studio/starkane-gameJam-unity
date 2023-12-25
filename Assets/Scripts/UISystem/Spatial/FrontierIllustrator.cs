using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using System.Collections.Generic;
using System;
using UnityEngine;
using Amegakure.Starkane.Id;
using Amegakure.Starkane.Context;

namespace Amegakure.Starkane.UI.Spatial
{
    [RequireComponent(typeof(TileRenderer))]
    public class FrontierIllustrator : MonoBehaviour
    {
        private TileRenderer tileRenderer;

        private void Awake()
        {
            tileRenderer = GetComponent<TileRenderer>();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.PATH_FRONTIERS_RESET, HandlePathFrontiersReset);

            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_UNSELECTED, HandleCharacterUnselected);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.PATH_FRONTIERS_RESET, HandlePathFrontiersReset);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_UNSELECTED, HandleCharacterUnselected);
        }

        private void HandleCharacterUnselected(Dictionary<string, object> context)
        {
            try
            {
                CharacterId id = (CharacterId)context["CharacterId"];
                GameObject characterGo = id.CharacterGo;
                WorldCharacterContext characterContext = characterGo.GetComponent<WorldCharacterContext>();
            
                Frontier frontierToClean = characterContext.Frontier;
                if(frontierToClean != null)
                    tileRenderer.ClearColor(frontierToClean.Tiles);
            }
            catch { }
        }

        private void HandleCharacterSelected(Dictionary<string, object> context)
        {
            try
            {
                CharacterId id = (CharacterId)context["CharacterId"];
                GameObject characterGo = id.CharacterGo;
                WorldCharacterContext characterContext = characterGo.GetComponent<WorldCharacterContext>();
                GridMovement movement = characterGo.GetComponent<GridMovement>();
                
                Frontier frontier  = movement.FindPaths(characterContext.Location);
                characterContext.Frontier = frontier;

                this.IllustrateFrontier(frontier);
            }
            catch { }
        }

        private void HandlePathFrontiersReset(Dictionary<string, object> context)
        {
            try
            {
                List<Tile> tiles = (List<Tile>)context["Tiles"];
                tileRenderer.ClearColor(tiles);
            }
            catch (Exception e) { Debug.LogError(e); }
        } 

        public void IllustrateFrontier(Frontier frontier)
        {
            tileRenderer.SetActiveTiles(frontier.Tiles);
        }
    }
}

