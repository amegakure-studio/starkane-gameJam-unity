using UnityEngine;

namespace Amegakure.Starkane.GridSystem
{
    public class StaticTileObject : MonoBehaviour
    {
        private Rigidbody rb;

        private void Awake()
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            collision.gameObject.TryGetComponent<Tile>(out Tile tile);
            if (tile != null)
            {
                tile.OccupyingObject = gameObject;
                rb.detectCollisions = false;
                rb.isKinematic = true;
            }
        }   
    }
}
