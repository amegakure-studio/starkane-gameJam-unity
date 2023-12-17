using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    void ClearGrid()
    {
        for (int i = transform.childCount; i >= transform.childCount; i--)
        {
            if (transform.childCount == 0)
                break;

            int c = Mathf.Clamp(i - 1, 0, transform.childCount);
            DestroyImmediate(transform.GetChild(c).gameObject);
        }
    }

    Vector2 DetermineTileSize(Bounds tileBounds)
    {
        // Works with the hexagonal tiles
        // return new Vector2((tileBounds.extents.x * 2) * 0.75f, (tileBounds.extents.z * 2));
        
        // To use the real size of the mesh
        return new Vector2((tileBounds.extents.x * 2) * 1f, (tileBounds.extents.z * 2));
    }

    public void GenerateGrid(GameObject tile, Vector2Int gridsize)
    {
        ClearGrid();
        Vector2 tileSize = DetermineTileSize(tile.GetComponent<MeshFilter>().sharedMesh.bounds);
        Vector3 position = transform.position;

        for (int x = 0; x < gridsize.x; x++)
        {
            for (int y = 0; y < gridsize.y; y++)
            {
                position.x = transform.position.x + tileSize.x * x;
                position.z = transform.position.z + tileSize.y * y;

                // Works with the hexagonal tiles
                //position.z += OffsetUnevenRow(x, tileSize.y);

                CreateTile(tile, position, new Vector2Int(x, y));
            }
        }
    }

    float OffsetUnevenRow(float x, float y)
    {
        return x % 2 == 0 ? y / 2 : 0f;
    }

    void CreateTile(GameObject t, Vector3 pos, Vector2Int id)
    {
        GameObject newTile = Instantiate(t.gameObject, pos, Quaternion.identity, transform);
        newTile.name = "Tile " + id;
        
        newTile.GetComponent<Tile>().coordinate = id;

        Debug.Log("Created a tile!");
    }
}
