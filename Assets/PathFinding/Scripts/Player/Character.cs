using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region member fields
    public bool Moving { get; private set; } = false;

    public CharacterMoveData movedata;
    public Tile characterTile;
    [SerializeField]
    LayerMask GroundLayerMask;
    private Animator animator;
    private Collider collider;
    #endregion

    private void Awake()
    {
        if (characterTile != null)
        {
            FinalizePosition(characterTile);
        }
        
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision collision)
    {
       if (characterTile == null)
       {
            Tile tile = null;
            collision.gameObject.TryGetComponent<Tile>(out tile);
            if(tile != null)
               FinalizePosition(tile); 
       }

       Physics.IgnoreCollision(collider, collision.collider);
    }

    IEnumerator MoveThroughPath(Path path)
    {
        int step = 0;
        int pathlength = Mathf.Clamp(path.tilesInPath.Length, 0, movedata.MaxMove + 1);
        Tile currentTile = path.tilesInPath[0];
        float animationtime = 0f;
        const float minimumistanceFromNextTile = 0.05f;

        while (step < pathlength)
        {
            yield return null;
            Vector3 nextTilePosition = path.tilesInPath[step].transform.position;

            MoveAndRotate(currentTile.transform.position, nextTilePosition, animationtime / movedata.MoveSpeed);
            animationtime += Time.deltaTime;

            if (Vector3.Distance(transform.position, nextTilePosition) > minimumistanceFromNextTile)
                continue;

            currentTile = path.tilesInPath[step];
            step++;
            animationtime = 0f;
        }

        FinalizePosition(path.tilesInPath[pathlength - 1]);
        animator.SetBool("IsWalking", false);
    }

    public void Move(Path _path)
    {
        Moving = true;
        characterTile.Occupied = false;
        animator.SetBool("IsWalking", true);
        StartCoroutine(MoveThroughPath(_path));
    }

    void FinalizePosition(Tile tile)
    {
        transform.position = tile.transform.position;
        characterTile = tile;
        Moving = false;
        tile.Occupied = true;
        tile.occupyingCharacter = this;
    }

    void MoveAndRotate(Vector3 origin, Vector3 destination, float duration)
    {
        transform.position = Vector3.Lerp(origin, destination, duration);
        transform.rotation = Quaternion.LookRotation(origin.DirectionTo(destination).Flat(), Vector3.up);
    }

}
