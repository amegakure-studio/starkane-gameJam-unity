using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathIllustrator))]
public class Pathfinder : MonoBehaviour
{
    #region member fields
    PathIllustrator illustrator;
    [SerializeField]
    LayerMask tileMask;

    Frontier currentFrontier = new Frontier();
    #endregion

    private void Start()
    {
        if (illustrator == null)
            illustrator = GetComponent<PathIllustrator>();
    }

    /// <summary>
    /// Main pathfinding function, marks tiles as being in frontier, while keeping a copy of the frontier
    /// in "currentFrontier" for later clearing
    /// </summary>
    /// <param name="character"></param>
    public void FindPaths(Character character)
    {
        ResetPathfinder();
        Tile currentTile = character.characterTile;
        currentTile.cost = 0;

        // FindAdjacentTilesArea(currentTile, 4);
    
        // List<Tile> adjacentTiles = createCrossPath(currentTile, 3);
        List<Tile> adjacentTiles = createDobuleSideUpDownStraightPath(currentTile, 2);

        foreach (Tile adjacentTile in adjacentTiles)
            AddTileToFrontier(adjacentTile);

        illustrator.IllustrateFrontier(currentFrontier);
    }

    public void FindAdjacentTilesArea(Tile tile, int maxDepth)
    {
        Queue<Tile> openSet = new Queue<Tile>();
        openSet.Enqueue(tile);
        while (openSet.Count > 0)
        {
            Tile currentTile = openSet.Dequeue();

            foreach (Tile adjacentTile in FindAdjacentTiles(currentTile))
            {
                if (openSet.Contains(adjacentTile))
                    continue;

                adjacentTile.cost = currentTile.cost + 1;

                if (!IsValidTile(adjacentTile, maxDepth))
                    continue;
    
                adjacentTile.parent = currentTile;

                openSet.Enqueue(adjacentTile);
                AddTileToFrontier(adjacentTile);
            }
        }
    }

    bool IsValidTile(Tile tile, int maxcost)
    {
        bool valid = false;

        if (!currentFrontier.tiles.Contains(tile) && tile.cost <= maxcost)
            valid = true;

        return valid;
    }

    void AddTileToFrontier(Tile tile)
    {
        tile.InFrontier = true;
        currentFrontier.tiles.Add(tile);
    }

    /// <summary>
    /// Returns a list of all neighboring hexagonal tiles and ladders
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    private List<Tile> FindAdjacentTiles(Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Vector3 direction = Vector3.forward;
        float rayLength = 50f;
        float rayHeightOffset = 1f;

        //Rotate a raycast in 60 degree steps and find all adjacent tiles
        for (int i = 0; i < 8; i++)
        {
            direction = Quaternion.Euler(0f, 45f, 0f) * direction;

            Vector3 aboveTilePos = (origin.transform.position + direction).With(y: origin.transform.position.y + rayHeightOffset);

            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                if (!hitTile.Occupied)
                    tiles.Add(hitTile);
            }
        }

        if (origin.connectedTile != null)
            tiles.Add(origin.connectedTile);

        return tiles;
    }

    private List<Tile> FindAdjacentTilesWithAngle(Tile origin, float initialAngle, int maxMove)
    {
        List<Tile> tiles = new();
        Vector3 direction = Vector3.forward;
        float rayLength = 50f;
        float rayHeightOffset = 1f;
        Tile tile = null;
        int actualMove = 0;

        while(actualMove < maxMove)
        {
            if (origin.connectedTile != null)
                tiles.Add(origin.connectedTile);

            direction = Quaternion.Euler(0f, initialAngle, 0f) * direction;
            Vector3 aboveTilePos = (origin.transform.position + direction).With(y: origin.transform.position.y + rayHeightOffset);
            
            // Debug.DrawRay(aboveTilePos, Vector3.down, Color.blue, 10);

            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                if (!hitTile.Occupied)
                {
                    tile = hitTile;
                    if(!this.currentFrontier.tiles.Contains(hitTile))
                    {
                        Debug.Log("aca");
                        tiles.Add(tile);
                    }
                    
                    tile.parent = null;

                    actualMove += 1;
                }
                else
                {
                    break;
                }

                origin = hitTile;   
            }
            else
                break;
            
            direction = Vector3.forward;
        }
        
        return tiles;
    }

    /// <summary>
    /// Called by Interact.cs to create a path between two tiles on the grid 
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public Path PathBetween(Tile dest, Tile source)
    {
        Path path = MakePath(dest, source);
        illustrator.IllustratePath(path);
        return path;
    }

    /// <summary>
    /// Creates a path between two tiles
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    private Path MakePath(Tile destination, Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Tile current = destination;

        while (current != origin)
        {
            tiles.Add(current);
            if (current.parent != null)
                current = current.parent;
            else
                break;
        }

        tiles.Add(origin);
        tiles.Reverse();

        Path path = new Path();
        path.tilesInPath = tiles.ToArray();

        return path;
    }

    public void ResetPathfinder()
    {
        illustrator.Clear();

        foreach (Tile item in currentFrontier.tiles)
        {
            item.InFrontier = false;
            item.ClearColor();
        }

        currentFrontier.tiles.Clear();
    }

    private List<Tile> createCrossPath(Tile origin, int maxDepth)
    {
        List<float> allowedAngles = new(){90f, 180f, 270f, 360f};
        List<Tile> result =  new();
        List<Tile> adjacentTiles =  new();
        
        foreach(float angle in allowedAngles)
        {
            adjacentTiles = FindAdjacentTilesWithAngle(origin, angle, maxDepth);
            foreach(Tile tile in adjacentTiles)
                result.Add(tile);
        }        

        return result;
    }


    private List<Tile> createDiagonalPath(Tile origin, int maxDepth)
    {
        List<float> allowedAngles = new(){45f, 135f, 225f, 315f};
        List<Tile> result =  new();
        List<Tile> adjacentTiles =  new();
        
        foreach(float angle in allowedAngles)
        {
            adjacentTiles = FindAdjacentTilesWithAngle(origin, angle, maxDepth);
            foreach(Tile tile in adjacentTiles)
                result.Add(tile);
        }        

        return result;
    }

    private List<Tile> createStraightPath(Tile origin, float angle, int maxDepth)
    {
        List<Tile> adjacentTiles =  new();
        
        return FindAdjacentTilesWithAngle(origin, angle, maxDepth);
    }

    private List<Tile> create90FStraightPath(Tile origin, int maxDepth)
    {
        List<Tile> adjacentTiles =  new();
        
        return FindAdjacentTilesWithAngle(origin, 90f, maxDepth);
    }

    private List<Tile> createUpDownStraightPath(Tile origin, int maxDepth)
    {
        List<float> allowedAngles = new(){180f, 360f};
        List<Tile> result =  new();
        List<Tile> adjacentTiles =  new();
        foreach(float angle in allowedAngles)
        {
            adjacentTiles = FindAdjacentTilesWithAngle(origin, angle, maxDepth/2);
            foreach(Tile tile in adjacentTiles)
                result.Add(tile);
        }        
        
        return result;
    }

    private List<Tile> createSideUpDownStraightPath(Tile origin, int maxDepth)
    {
        List<Tile> tiles = create90FStraightPath(origin, 1);
        Tile tile = null;
        foreach (Tile _tile in tiles)
            tile = _tile;
    
        if(tile != null)
        {
            tile.cost = 0;   
            tile.parent = origin;
            List<Tile> adjacentTiles = createUpDownStraightPath(tile, maxDepth);
            if(adjacentTiles.Count < 0)
                return tiles;
            else
            {
                adjacentTiles.Add(tile);
                return adjacentTiles;
            }
        }
        return tiles;
    }


    private List<Tile> createDobuleSideUpDownStraightPath(Tile origin, int maxDepth)
    {
        // TODO: Verify parents!
        List<Tile> tiles = create90FStraightPath(origin, 1);
        Tile tile = null;
        foreach (Tile _tile in tiles)
            tile = _tile;
        
        if(tile != null)
        {
            List<Tile> adjacentTiles =  createSideUpDownStraightPath(tile, maxDepth);
            adjacentTiles.Add(tile);
            return adjacentTiles; 
        }
        else
            return tiles;
    }


}
