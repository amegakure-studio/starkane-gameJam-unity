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
            tileRenderer = GetComponent<TileRenderer>();
        }

        private void OnMouseOver()
        {
            if (tile && tile.InFrontier && tileRenderer)
                tileRenderer.SetColor(TileColor.Highlighted);
        }

        private void OnMouseExit()
        {
            if (tileRenderer)
            {
                if (tile && tile.InFrontier)
                    tileRenderer.SetColor(TileColor.Green);
                else
                    tileRenderer.ClearColor();
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
