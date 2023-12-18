using UnityEngine;

public class CustomObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.TryGetComponent<Tile>(out Tile tile);
        if (tile != null)
        {
            tile.Occupied = true;
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.detectCollisions = false;
            rb.isKinematic = true;
        }
    }

    private void Start()
    {
        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
    }
}
