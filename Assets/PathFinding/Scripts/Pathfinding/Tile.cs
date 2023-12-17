using System.Collections.Generic;
using UnityEngine;

public enum TileColor { Green, Highlighted };

public class Tile : MonoBehaviour
{
    #region member fields
    public Tile parent;
    public Tile connectedTile;
    public Character occupyingCharacter;
    public float cost;
    public Vector2Int coordinate;

    [SerializeField]
    GameObject GreenChild, WhiteChild;

    [SerializeField]
    bool debug;

    public bool Occupied { get; set; } = false;
    public bool InFrontier { get; set; } = false;
    public bool CanBeReached { get { return !Occupied && InFrontier; } }

    private List<Tile> neighbors = null;
    #endregion

    /// <summary>
    /// Changes color of the tile by activating child-objects of different colors
    /// </summary>
    /// <param name="col"></param>
    public void SetColor(TileColor col)
    {

        ClearColor();

        switch (col)
        {
            case TileColor.Green:
                GreenChild.SetActive(true);
                DebugWithArrow();
                break;
            case TileColor.Highlighted:
                WhiteChild.SetActive(true);
                break;
            default:
                break;
        }
    }

    void DebugWithArrow()
    {
        Transform childArrow = GreenChild.transform.GetChild(0);

        if (!debug) 
            childArrow.gameObject.SetActive(false);

        if(childArrow != null && parent != null)
            childArrow.rotation = Quaternion.LookRotation(parent.transform.position - transform.position, Vector3.up);
    }

    /// <summary>
    /// Deactivates all children, removing all color
    /// </summary>
    public void ClearColor()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public List<Tile> GetNeighbors()
    {
        //     dirs = [[1, 0], [1,1] [0, 1], [-1, 1], [-1, 0], [-1, -1], [0, -1], [1,-1]]
        List<Vector2> directions = new List<Vector2>
        {
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(-1, 1),
            new Vector2(-1, 0),
            new Vector2(-1, -1),
            new Vector2(0, -1),
            new Vector2(1, -1)
        };

        if(this.neighbors == null)
            this.neighbors = this.CalculateNeighbors(directions);
        
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

        foreach(Vector2 direction in directions)
        {
            neighborsDir.Add(coordinate + direction);
        }

        GameObject parent = transform.parent.gameObject;
        
        Tile[] tiles = parent.GetComponentsInChildren<Tile>();

        foreach(Tile tile in tiles)
        {
            if(neighborsDir.Contains(tile.coordinate))
                neighbors.Add(tile);

            if (neighbors.Count == 8)
                    break;
        }

        if(this.connectedTile != null)
            neighbors.Add(connectedTile);
        
        return neighbors;
    }
}
