using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using System.Collections.Generic;
using System;
using UnityEngine;
using Amegakure.Starkane.EntitiesWrapper;

namespace Amegakure.Starkane.UI.Spatial
{
    [RequireComponent(typeof(TileRenderer))]
    public class FrontierIllustrator : MonoBehaviour
    {
        private TileRenderer tileRenderer;
        private GridManager gridManager;

        private void Awake()
        {
            tileRenderer = GetComponent<TileRenderer>();
            gridManager = FindAnyObjectByType<GridManager>();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.PATH_FRONTIERS_RESET, HandlePathFrontiersReset);

            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_UNSELECTED, HandleCharacterUnselected);
            
            EventManager.Instance.Subscribe(GameEvent.FRONTIER_UPDATED, HandleFrontierUpdated);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.PATH_FRONTIERS_RESET, HandlePathFrontiersReset);

            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_UNSELECTED, HandleCharacterUnselected);

            EventManager.Instance.Unsubscribe(GameEvent.FRONTIER_UPDATED, HandleFrontierUpdated);
        }

        private void HandleFrontierUpdated(Dictionary<string, object> context)
        {
            try
            {
                Frontier frontier = (Frontier)context["Frontier"];
                this.IllustrateFrontier(frontier);
            }
            catch { }
        }

        private void HandleCharacterUnselected(Dictionary<string, object> context)
        {
            try
            {
                Character character = (Character)context["Character"];

                Frontier frontierToClean = character.GetMovementFrontier();
                if(frontierToClean != null)
                    tileRenderer.ClearColor(frontierToClean.Tiles);
            }
            catch { }
        }

        private void HandleCharacterSelected(Dictionary<string, object> context)
        {
            try
            {
                tileRenderer.ClearColor(gridManager.TilesInMap);
                
                Character character = (Character)context["Character"];
                Frontier frontier  = character.GetMovementFrontier();

                this.IllustrateFrontier(frontier);
            }
            catch { }
        }

        private void HandlePathFrontiersReset(Dictionary<string, object> context)
        {
            if(context == null)
            {
                tileRenderer.ClearColor(gridManager.TilesInMap);
            }
            else
            {
                try
                {
                    List<Tile> tiles = (List<Tile>)context["Tiles"];
                    tileRenderer.ClearColor(tiles);
                }
                catch (Exception e) { Debug.LogError(e); }    
            }
            
        } 

        public void IllustrateFrontier(Frontier frontier)
        {
            tileRenderer.SetActiveTiles(frontier.Tiles);
        }
    }
}

