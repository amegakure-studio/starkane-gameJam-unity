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
        [SerializeField] GameObject obstaclePrefab;

        private WorldManager worldManager;
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
            worldManager = GameObject.FindAnyObjectByType<WorldManager>();

            if (worldManager != null)
                worldManager.OnEntityFeched += WorldManager_OnEntityFeched;
        }

        
        private void OnDisable()
        {
            if(worldManager != null)
                worldManager.OnEntityFeched -= WorldManager_OnEntityFeched;
        }

        private void WorldManager_OnEntityFeched(WorldManager obj)
        {
            GameObject[] entities = worldManager.Entities();

            foreach (GameObject go in entities)
            {
                try
                {
                    if (obstaclePrefab == null)
                        return;

                    MapCC mapCC = go.GetComponent<MapCC>();

                    if (mapCC != null )
                    {
                        foreach(Tile tile in tilesInMap)
                        {
                            if (!mapCC.IsWalkable(tile.coordinate))
                            {
                                GameObject obstacle = Instantiate(obstaclePrefab);
                                obstacle.transform.parent = tile.transform;
                                
                                obstacle.transform.localPosition = Vector3.zero;
                                tile.OccupyingObject = obstacle;
                                // obstacle.transform.localRotation = Quaternion.identity;
                                // obstacle.transform.localScale = new Vector3(1,1,1);
                            }
                        }
                    }
                }
                catch(Exception e) { Debug.LogError(e); }
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
