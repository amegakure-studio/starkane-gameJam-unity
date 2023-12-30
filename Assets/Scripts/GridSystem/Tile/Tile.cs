using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.GridSystem
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] TileType tileType = TileType.ORTHOGONAL;

        #region member fields
        private Tile parent;
        private Tile connectedTile;
        private GameObject occupyingObject;
        private float cost;
        public Vector2Int coordinate;
        private List<Vector2> directions;
        private List<Tile> neighbors = null;
        private bool isMovementTile = true;
        #endregion

        private void Awake()
        {
            switch (tileType) 
            {
                case TileType.ORTHOGONAL: 
                    directions = new()
                    {
                        new Vector2(1, 0),
                        new Vector2(0, 1),
                        new Vector2(-1, 0),
                        new Vector2(0, -1),
                    }; break;
                case TileType.HEXAGONAL:
                    directions = new()
                    {
                        new Vector2(1, 0),
                        new Vector2(1, 1),
                        new Vector2(0, 1),
                        new Vector2(-1, 1),
                        new Vector2(-1, 0),
                        new Vector2(-1, -1),
                        new Vector2(0, -1),
                        new Vector2(1, -1)
                    }; break;
            }
        }

        public GameObject OccupyingObject { get => occupyingObject; set => occupyingObject = value; }
        public Vector2Int Coordinate { get => coordinate; set => coordinate = value; }
        public bool Occupied() { return occupyingObject != null; }
        public bool InFrontier { get; set; } = false;
        public bool CanBeReached { get { return !Occupied() && InFrontier; } }
        public float Cost { get => cost; set => cost = value; }
        public Tile Parent { get => parent; set => parent = value; }
        public Tile ConnectedTile { get => connectedTile; set => connectedTile = value; }
        public bool IsMovementTile { get => isMovementTile; set => isMovementTile = value; }

        public List<Tile> GetNeighbors()
        {
            this.neighbors ??= this.CalculateNeighbors(directions);
            return neighbors;
        }


        public List<Tile> GetNeighbors(List<Vector2> directions)
        {
            return this.CalculateNeighbors(directions);
        }

        public List<Tile> CalculateNeighbors(List<Vector2> directions)
        {
            List<Tile> neighbors = new();

            List<Vector2> neighborsDir = new();

            foreach (Vector2 direction in directions)
            {
                neighborsDir.Add(coordinate + direction);
            }

            GameObject parent = transform.parent.gameObject;

            Tile[] tiles = parent.GetComponentsInChildren<Tile>();

            foreach (Tile tile in tiles)
            {
                if (neighborsDir.Contains(tile.coordinate))
                    neighbors.Add(tile);

                if (neighbors.Count == 8)
                    break;
            }

            if (this.connectedTile != null)
                neighbors.Add(connectedTile);

            return neighbors;
        }
    }
}

