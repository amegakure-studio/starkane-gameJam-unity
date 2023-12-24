using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.UI.Spatial;
using System.Collections.Generic;
using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    [SerializeField]
    Material Active, Highlighted;

    public void SetActiveTiles(List<Tile> tiles)
    {
        SetColor(tiles, TileColor.Active);
    }

    public void SetHighlightTiles(List<Tile> tiles)
    {
        SetColor(tiles, TileColor.Highlighted);
    }

    public void ClearColor(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            MeshRenderer meshRenderer = tile.gameObject.GetComponent<MeshRenderer>();
            ClearColor(meshRenderer);
        }
    }

    /// <summary>
    /// Changes color of the tile by changing tile material
    /// </summary>
    /// <param name="col"></param>
    private void SetColor(List<Tile> tiles, TileColor col)
    {
        foreach (Tile tile in tiles)
        {
            MeshRenderer meshRenderer = tile.gameObject.GetComponent<MeshRenderer>();
            meshRenderer.enabled = true;

            switch (col)
            {
                case TileColor.Active:
                    meshRenderer.material = Active;
                    break;
                case TileColor.Highlighted:
                    meshRenderer.material = Highlighted;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Set tile color to default
    /// </summary>
    private void ClearColor(MeshRenderer meshRenderer)
    {
        meshRenderer.enabled = false;
    }
}
