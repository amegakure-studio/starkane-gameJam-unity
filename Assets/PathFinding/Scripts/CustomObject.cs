using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomObject : MonoBehaviour
{
    [SerializeField] LayerMask GroundLayerMask;

    void Start()
    {
        Vector3[] edges = GetEdges();

        if (edges != null)
        {
            foreach (Vector3 edge in edges)
            {
                float rayLength = 50f;
                
                if (Physics.Raycast(edge, -transform.up, out RaycastHit hit, rayLength, GroundLayerMask))
                {
                    try
                    {
                        Tile tile = hit.transform.GetComponent<Tile>();
                        tile.Occupied = true;
                    }
                    catch{}
                    // Debug.DrawRay(edge, -transform.up * rayLength, Color.green, 100);
                }
            }
        }
    }

    private Vector3[] GetEdges()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            Vector3 colliderCenter = boxCollider.bounds.center;
            Vector3 colliderExtents = boxCollider.bounds.extents;

            // Calculate edges
            Vector3[] edges = new Vector3[8];

            edges[0] = colliderCenter + new Vector3(colliderExtents.x, colliderExtents.y, colliderExtents.z);
            edges[1] = colliderCenter + new Vector3(colliderExtents.x, colliderExtents.y, -colliderExtents.z);
            edges[2] = colliderCenter + new Vector3(-colliderExtents.x, colliderExtents.y, colliderExtents.z);
            edges[3] = colliderCenter + new Vector3(-colliderExtents.x, colliderExtents.y, -colliderExtents.z);

            edges[4] = colliderCenter + new Vector3(colliderExtents.x, -colliderExtents.y, colliderExtents.z);
            edges[5] = colliderCenter + new Vector3(colliderExtents.x, -colliderExtents.y, -colliderExtents.z);
            edges[6] = colliderCenter + new Vector3(-colliderExtents.x, -colliderExtents.y, colliderExtents.z);
            edges[7] = colliderCenter + new Vector3(-colliderExtents.x, -colliderExtents.y, -colliderExtents.z);

            // return edges;

            // Interpolate additional points between each pair of existing edges
            int interpolationPoints = 10;
            Vector3[] interpolatedEdges = new Vector3[edges.Length + edges.Length * interpolationPoints];

            for (int i = 0; i < edges.Length; i++)
            {
                interpolatedEdges[i] = edges[i];

                // Interpolate points between the current edge and the next edge
                for (int j = 1; j <= interpolationPoints; j++)
                {
                    float t = j / (float)(interpolationPoints + 1);
                    interpolatedEdges[i + j + (i * interpolationPoints)] = Vector3.Lerp(edges[i], edges[(i + 1) % edges.Length], t);
                }
            }

            return interpolatedEdges;
        }
        return null;
    }
}
