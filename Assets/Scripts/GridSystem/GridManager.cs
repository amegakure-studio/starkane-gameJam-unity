using System;
using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.GridSystem;
using Dojo;
using UnityEngine;

namespace Amegakure.Starkane.GridSystem
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] WorldManager worldManager;
        [SerializeField] GameObject obstaclePrefab;
        private Dictionary<Vector2, Tile> worldMap;
        private List<Tile> tilesInMap;
        public Dictionary<Vector2, Tile> WorldMap { get => worldMap; private set => worldMap = value; }
        public List<Tile> TilesInMap { get => tilesInMap; private set => tilesInMap = value; }

        void Awake()
        {
            Tile[] tiles = GetComponentsInChildren<Tile>();
            tilesInMap = new();
            worldMap = new();
            
            foreach (Tile tile in tiles )
            {
                WorldMap[tile.Coordinate] = tile;
                tilesInMap.Add(tile);
            }
        }

        private void OnEnable()
        {
            worldManager.OnEntityFeched += WorldManager_OnEntityFeched;
        }

        
        private void OnDisable()
        {
            worldManager.OnEntityFeched -= WorldManager_OnEntityFeched;
        }

        private void WorldManager_OnEntityFeched(WorldManager obj)
        {
            GameObject[] entities = worldManager.Entities();

            foreach (GameObject go in entities)
            {
                try
                {
                    MapCC mapCC = go.GetComponent<MapCC>();

                    if (mapCC != null )
                    {
                        foreach(Tile tile in tilesInMap)
                        {
                            if (!mapCC.IsWalkable(tile.coordinate))
                            {
                                GameObject obstacle = Instantiate(obstaclePrefab, tile.transform.position, Quaternion.identity);
                                obstacle.transform.parent = tile.transform;
                                obstacle.transform.localPosition = Vector3.zero;
                            }
                        }
                    }
                }
                catch { }
            }

            throw new ArgumentException("Couldn't get character entity");
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
