using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathIllustrator : MonoBehaviour
{
    private const float LineHeightOffset = 0.33f;
    LineRenderer line;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.PATH_FRONTIERS_UPDATED, HandlePathFrontiersUpdated);
        EventManager.Instance.Subscribe(GameEvent.PATH_UPDATED, HandlePathUpdated);
        EventManager.Instance.Subscribe(GameEvent.PATH_RESET, HandlePathDeleted);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.PATH_FRONTIERS_UPDATED, HandlePathFrontiersUpdated);
        EventManager.Instance.Unsubscribe(GameEvent.PATH_UPDATED, HandlePathUpdated);
        EventManager.Instance.Unsubscribe(GameEvent.PATH_RESET, HandlePathDeleted);
    }

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void HandlePathDeleted(Dictionary<string, object> context) { Clear(); }

    private void HandlePathUpdated(Dictionary<string, object> context)
    {
        try
        {
            Path path = (Path)context["Path"];
            IllustratePath(path);
        }
        catch (Exception e) { Debug.LogError(e); }
    }

    private void HandlePathFrontiersUpdated(Dictionary<string, object> context)
    {
        try
        {
            Frontier frontier = (Frontier) context["Frontier"];
            IllustrateFrontier(frontier);
        }
        catch (Exception e) { Debug.LogError(e); }
    }

    public void IllustratePath(Path path)
    {
        line.positionCount = path.tilesInPath.Length;

        for (int i = 0; i < path.tilesInPath.Length; i++)
        {
            Transform tileTransform = path.tilesInPath[i].transform;
            line.SetPosition(i, tileTransform.position.With(y: tileTransform.position.y + LineHeightOffset));
        }
    }

    public void IllustrateFrontier(Frontier frontier)
    {
        foreach (Tile item in frontier.tiles)
        {
            item.SetColor(TileColor.Green);
        }
    }

    public void Clear()
    {
        line.positionCount = 0;
    }

}
