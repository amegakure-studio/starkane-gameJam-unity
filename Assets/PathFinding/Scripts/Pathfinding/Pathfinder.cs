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

        List<Tile> adjacentTiles = new ();

        adjacentTiles =  GetNeighborsInArea(currentTile, 3);
        foreach (Tile adjacentTile in adjacentTiles)
            AddTileToFrontier(adjacentTile);

        
        // adjacentTiles = GetNeighborsWithAngle(currentTile, 3, new Vector2(-1,0));
        // adjacentTiles = createCrossPath(currentTile, 4);
        // adjacentTiles = createDiagonalPath(currentTile, 2);
        // adjacentTiles = create90FStraightPath(currentTile, 2);
        // adjacentTiles = createSideStraightPath(currentTile, 4);
        // adjacentTiles = createStepSideStraightPath(currentTile, 2);

        // foreach (Tile adjacentTile in adjacentTiles)
        //     AddTileToFrontier(adjacentTile);

        illustrator.IllustrateFrontier(currentFrontier);
    }


    public List<Tile> GetNeighborsInArea(Tile tile, int maxDepth)
    {
        List<Tile> tiles = new();
        Queue<Tile> openSet = new Queue<Tile>();
        openSet.Enqueue(tile);
        while (openSet.Count > 0)
        {
            Tile currentTile = openSet.Dequeue();

            foreach (Tile adjacentTile in currentTile.GetNeighbors())
            {
                if (openSet.Contains(adjacentTile) || tiles.Contains(adjacentTile))
                    continue;

                adjacentTile.cost = currentTile.cost + 1;

                if (!IsValidTile(adjacentTile, maxDepth))
                    continue;

                adjacentTile.parent = currentTile;

                openSet.Enqueue(adjacentTile);
                tiles.Add(adjacentTile);
                // AddTileToFrontier(adjacentTile);
            }
        }
        return tiles;
    }

    public List<Tile> GetNeighborsWithAngle(Tile origin, int maxDepth, Vector2 direction)
    {
        List<Tile> tiles = new();
        int actualMove = 0;
        Tile actualTile = origin;
        
        while(actualMove < maxDepth)
        {
            List<Tile> neighbors = actualTile.GetNeighbors(new List<Vector2>(){direction});
            
            if(neighbors.Count == 0)
                break;
            
            if (!neighbors[0].Occupied)
            {
                neighbors[0].parent = null;
                tiles.Add(neighbors[0]);
                actualMove += 1;
            }
            else
            {
                break;
            }

            actualTile = neighbors[0];  
        }

        return tiles;
    }

    bool IsValidTile(Tile tile, int maxcost)
    {
        bool valid = false;

        if (tile.cost <= maxcost && !tile.Occupied)
            valid = true;

        return valid;
    }

    void AddTileToFrontier(Tile tile)
    {
        if(!currentFrontier.tiles.Contains(tile))
        {
            tile.InFrontier = true;
            currentFrontier.tiles.Add(tile);
        }
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

    private List<Tile> createCustomPath(Tile origin, int maxDepth, List<Vector2> directions)
    {
        List<Tile> result =  new();
        List<Tile> adjacentTiles =  new();

        foreach(Vector2 direction in directions)
        {
            adjacentTiles = GetNeighborsWithAngle(origin, maxDepth, direction);
            
            foreach(Tile tile in adjacentTiles)
                result.Add(tile);
        }

        return result;
    }


    // Make this path: (where c is the character)
    //  -
    // -C-
    //  -
    private List<Tile> createCrossPath(Tile origin, int maxDepth)
    {
        List<Vector2> directions = new List<Vector2>
        {
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(-1, 0),
            new Vector2(0, -1)
        };

        return createCustomPath(origin, maxDepth, directions);
    }


    // Make this path: (where c is the character)
    // - -
    //  C
    // - -
    private List<Tile> createDiagonalPath(Tile origin, int maxDepth)
    {
        List<Vector2> directions = new List<Vector2>
        {
            new Vector2(1, 1),
            new Vector2(-1, 1),
            new Vector2(-1, -1),
            new Vector2(1, -1)
        };

        return createCustomPath(origin, maxDepth, directions);
    }


    // Make this path: (where c is the character)
    //  
    // C- (maxDepth time)
    private List<Tile> create90FStraightPath(Tile origin, int maxDepth)
    {
        List<Vector2> directions = new List<Vector2>
        {
            new Vector2(1, 0)
        };

        return createCustomPath(origin, maxDepth, directions);
    }

    // Make this path: (where c is the character)
    //  -
    // C-
    //  -
    private List<Tile> createSideStraightPath(Tile origin, int maxDepth)
    {
        List<Tile> results = new();

        List<Tile> originNeighborns = origin.GetNeighbors(new List<Vector2>{new Vector2(1,0)});
        
        if(originNeighborns.Count > 0)
        {
            originNeighborns[0].parent = origin;
            results.Add(originNeighborns[0]);
            List<Vector2> directions = new List<Vector2>
            {
                new Vector2(0, 1),
                new Vector2(0, -1)
            };

            foreach(Tile tile in createCustomPath(originNeighborns[0], maxDepth/2, directions))
            {
                tile.parent = originNeighborns[0];
                results.Add(tile);
            }
                
        }
        
        return results;
    }

    // Make this path: (where c is the character)
    //   -
    // C--
    //   -
    private List<Tile> createStepSideStraightPath(Tile origin, int maxDepth)
    {
        List<Tile> results = new();

        List<Tile> originNeighborns = origin.GetNeighbors(new List<Vector2>{new Vector2(1,0)});
        
        if(originNeighborns.Count > 0)
        {
            results.Add(originNeighborns[0]);
            foreach(Tile tile in createSideStraightPath(originNeighborns[0], maxDepth))
            {
                // tile.parent = originNeighborns[0];
                results.Add(tile);
            }
        }

        return results;
    }
}