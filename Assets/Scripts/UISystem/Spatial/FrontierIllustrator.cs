using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using System.Collections.Generic;
using System;
using UnityEngine;

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
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.PATH_FRONTIERS_RESET, HandlePathFrontiersReset);
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

