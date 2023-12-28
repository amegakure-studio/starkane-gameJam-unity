using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.GridSystem;
using UnityEngine;

namespace Amegakure.Starkane.GridSystem
{
    public class GridManager : MonoBehaviour
    {
        private Dictionary<Vector2, Tile> worldMap;
        public Dictionary<Vector2, Tile> WorldMap { get => worldMap; private set => worldMap = value; }

        void Awake()
        {
            Tile[] tiles = GetComponentsInChildren<Tile>();
            worldMap = new();
            
            foreach (Tile tile in tiles )
            {
                WorldMap[tile.Coordinate] = tile;
            }
        }

        public List<Tile> GetFreeTiles()
        {
            List<Tile> tiles = new();
            
            foreach(Vector2 key in worldMap.Keys)
            {
                Tile tile = worldMap[key];
                
                if(!tile.Occupied())
                    tiles.Add(tile);
            }
            
            return tiles;    
        }
    }
}
