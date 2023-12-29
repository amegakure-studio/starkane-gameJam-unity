using Amegakure.Starkane.PubSub;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.GridSystem
{
    public class Pathfinder
    {
        #region member fields

        Frontier currentFrontier = new();

        #endregion

        public Frontier FindPaths(Tile target, int maxTilesDepth, PathStyle tileStyle, bool isMovementTile)
        {
            ResetPathfinder();
            Tile currentTile = target;

            if (currentTile != null)
            {
                currentTile.Cost = 0;

                List<Tile> adjacentTiles = new();

                switch (tileStyle)
                {
                    case PathStyle.SQUARE:
                        adjacentTiles = GetNeighborsInArea(currentTile, maxTilesDepth); break;
                    case PathStyle.CROSS:
                        adjacentTiles = CreateCrossPath(currentTile, maxTilesDepth); break;
                    case PathStyle.DIAGONAL:
                        adjacentTiles = CreateDiagonalPath(currentTile, maxTilesDepth); break;
                    case PathStyle.FORWARD:
                        adjacentTiles = Create90FStraightPath(currentTile, maxTilesDepth); break;
                    case PathStyle.VERTICAL:
                        adjacentTiles = CreateSideStraightPath(currentTile, maxTilesDepth); break;
                    case PathStyle.FORWARD_VERTICAL:
                        adjacentTiles = CreateStepSideStraightPath(currentTile, maxTilesDepth); break;
                }

                foreach (Tile adjacentTile in adjacentTiles)
                {                    
                    if (isMovementTile)
                    {
                        adjacentTile.IsMovementTile = isMovementTile;

                        if (!adjacentTile.Occupied())
                            AddTileToFrontier(adjacentTile);
                    }
                    else
                    {
                        AddTileToFrontier(adjacentTile);
                    }
                }
            }   

            return currentFrontier;
        }

        public bool IsTileInFrontier(Tile tile)
        {
            return currentFrontier.Tiles.Contains(tile);
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

                    adjacentTile.Cost = currentTile.Cost + 1;

                    if (!IsValidTile(adjacentTile, maxDepth))
                        continue;

                    adjacentTile.Parent = currentTile;

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

            while (actualMove < maxDepth)
            {
                List<Tile> neighbors = actualTile.GetNeighbors(new List<Vector2>() { direction });

                if (neighbors.Count == 0)
                    break;

                neighbors[0].Parent = null;
                tiles.Add(neighbors[0]);
                actualMove += 1;

                actualTile = neighbors[0];
            }

            return tiles;
        }

        bool IsValidTile(Tile tile, int maxcost)
        {
            bool valid = false;

            if (tile.Cost <= maxcost && !tile.Occupied())
                valid = true;

            return valid;
        }

        void AddTileToFrontier(Tile tile)
        {
            if (!currentFrontier.Tiles.Contains(tile))
            {
                tile.InFrontier = true;
                currentFrontier.Tiles.Add(tile);
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
                if (current.Parent != null)
                    current = current.Parent;
                else
                    break;
            }

            tiles.Add(origin);
            tiles.Reverse();

            Path path = new();
            path.TilesInPath = tiles.ToArray();

            return path;
        }

        public void ResetPathfinder()
        {
            currentFrontier.Tiles.ForEach(tile => tile.InFrontier = false);

            EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET, new() { { "Tiles", currentFrontier.Tiles } });
            currentFrontier.Tiles.Clear();
        }

        private List<Tile> CreateCustomPath(Tile origin, int maxDepth, List<Vector2> directions)
        {
            List<Tile> result = new();
            List<Tile> adjacentTiles = new();

            foreach (Vector2 direction in directions)
            {
                adjacentTiles = GetNeighborsWithAngle(origin, maxDepth, direction);

                foreach (Tile tile in adjacentTiles)
                    result.Add(tile);
            }

            return result;
        }


        // Make this path: (where c is the character)
        //  -
        // -C-
        //  -
        private List<Tile> CreateCrossPath(Tile origin, int maxDepth)
        {
            List<Vector2> directions = new List<Vector2>
        {
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(-1, 0),
            new Vector2(0, -1)
        };

            return CreateCustomPath(origin, maxDepth, directions);
        }


        // Make this path: (where c is the character)
        // - -
        //  C
        // - -
        private List<Tile> CreateDiagonalPath(Tile origin, int maxDepth)
        {
            List<Vector2> directions = new List<Vector2>
        {
            new Vector2(1, 1),
            new Vector2(-1, 1),
            new Vector2(-1, -1),
            new Vector2(1, -1)
        };

            return CreateCustomPath(origin, maxDepth, directions);
        }


        // Make this path: (where c is the character)
        //  
        // C- (maxDepth time)
        private List<Tile> Create90FStraightPath(Tile origin, int maxDepth)
        {
            List<Vector2> directions = new List<Vector2>
        {
            new Vector2(1, 0)
        };

            return CreateCustomPath(origin, maxDepth, directions);
        }

        // Make this path: (where c is the character)
        //  -
        // C-
        //  -
        private List<Tile> CreateSideStraightPath(Tile origin, int maxDepth)
        {
            List<Tile> results = new();

            List<Tile> originNeighborns = origin.GetNeighbors(new List<Vector2> { new Vector2(1, 0) });

            if (originNeighborns.Count > 0)
            {
                originNeighborns[0].Parent = origin;
                results.Add(originNeighborns[0]);
                List<Vector2> directions = new List<Vector2>
            {
                new Vector2(0, 1),
                new Vector2(0, -1)
            };

                foreach (Tile tile in CreateCustomPath(originNeighborns[0], maxDepth / 2, directions))
                {
                    tile.Parent = originNeighborns[0];
                    results.Add(tile);
                }

            }

            return results;
        }

        // Make this path: (where c is the character)
        //   -
        // C--
        //   -
        private List<Tile> CreateStepSideStraightPath(Tile origin, int maxDepth)
        {
            List<Tile> results = new();

            List<Tile> originNeighborns = origin.GetNeighbors(new List<Vector2> { new Vector2(1, 0) });

            if (originNeighborns.Count > 0)
            {
                results.Add(originNeighborns[0]);
                foreach (Tile tile in CreateSideStraightPath(originNeighborns[0], maxDepth))
                {
                    // tile.parent = originNeighborns[0];
                    results.Add(tile);
                }
            }

            return results;
        }
    }
}