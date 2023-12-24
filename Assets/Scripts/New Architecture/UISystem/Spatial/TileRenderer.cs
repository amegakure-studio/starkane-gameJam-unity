using Amegakure.Starkane.PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    [SerializeField]
    GameObject GreenChild, WhiteChild;

    private Tile tile;

    private void Awake()
    {
        tile = GetComponent<Tile>();
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.PATH_FRONTIERS_RESET, HandlePathFrontiersReset);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.PATH_FRONTIERS_RESET, HandlePathFrontiersReset);
    }

    private void HandlePathFrontiersReset(Dictionary<string, object> context)
    {
        try
        {
            List<Tile> tiles = (List<Tile>)context["Tiles"];

            if (tiles.Contains(tile))
                ClearColor();

        } catch (Exception e) { Debug.LogError(e); }        
    }

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
                DeactivateArrow();
                break;
            case TileColor.Highlighted:
                WhiteChild.SetActive(true);
                break;
            default:
                break;
        }
    }

    void DeactivateArrow()
    {
        Transform childArrow = GreenChild.transform.GetChild(0);
        childArrow.gameObject.SetActive(false);
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
}
