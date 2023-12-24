using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.GridSystem
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] GameObject tile;
        [SerializeField] int width;
        [SerializeField] int length;
        Vector2Int gridSize;

        private GameObject parent;
        [SerializeField] Vector3 gridPosition;
        // Start is called before the first frame update

        void Start()
        {
            gridSize = new Vector2Int(width, length);
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            TileGenerator tileGenerator;
            AssignGridParent();

            if (!parent.GetComponent<TileGenerator>())
                tileGenerator = parent.AddComponent<TileGenerator>();
            else
                tileGenerator = parent.GetComponent<TileGenerator>();

            tileGenerator.GenerateGrid(tile, gridSize);
        }

        void AssignGridParent()
        {
            if (parent == null)
                parent = new GameObject("Grid");

            parent.transform.position = gridPosition;
        }
    }
}
