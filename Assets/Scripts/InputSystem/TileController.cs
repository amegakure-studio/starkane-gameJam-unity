using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.InputSystem
{
    public class TileController : MonoBehaviour
    {
        private Tile tile;
        private TileRenderer tileRenderer;

        private void Awake()
        {
            tile = GetComponent<Tile>();
        }

        private void Start()
        {
            tileRenderer = GameObject.FindObjectOfType<TileRenderer>();
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
                Dictionary<string, object> context = new(){ { "Tile", this } };

                EventManager.Instance.Publish(GameEvent.TILE_SELECTED, context);
            }
        }
    }
}
