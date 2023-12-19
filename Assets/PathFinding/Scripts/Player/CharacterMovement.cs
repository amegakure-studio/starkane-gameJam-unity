using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    #region member fields
    public bool Moving { get; private set; } = false;
    public Tile Location { get => m_Location; set => m_Location = value; }

    [SerializeField] CharacterMoveData moveData;
    private Tile m_Location;
    private Pathfinder pathfinder;
    private Animator m_Animator;
    private Collider m_Collider;
    #endregion

    private void Awake()
    {       
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();
    }
    private void Start()
    {
        if (pathfinder == null)
            pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
    }

    void OnCollisionEnter(Collision collision)
    {
       if (Location == null)
       {
            collision.gameObject.TryGetComponent<Tile>(out Tile tile);

            if (tile != null)
               FinalizePosition(tile); 
       }

       Physics.IgnoreCollision(m_Collider, collision.collider);
    }

    IEnumerator MoveThroughPath(Path path)
    {
        int step = 0;
        int pathlength = Mathf.Clamp(path.tilesInPath.Length, 0, moveData.MaxMove + 1);
        Tile currentTile = path.tilesInPath[0];
        float animationtime = 0f;
        const float minimumistanceFromNextTile = 0.05f;

        while (step < pathlength)
        {
            yield return null;
            Vector3 nextTilePosition = path.tilesInPath[step].transform.position;

            MoveAndRotate(currentTile.transform.position, nextTilePosition, animationtime / moveData.MoveSpeed);
            animationtime += Time.deltaTime;

            if (Vector3.Distance(transform.position, nextTilePosition) > minimumistanceFromNextTile)
                continue;

            currentTile = path.tilesInPath[step];
            step++;
            animationtime = 0f;
        }

        FinalizePosition(path.tilesInPath[pathlength - 1]);
        m_Animator.SetBool("IsWalking", false);       
    }

    public void Move(Tile target)
    {
        if (CanReachTile(target)) 
        { 
            Path path = pathfinder.PathBetween(target, Location);

            Moving = true;
            Location.Occupied = false;
            m_Animator.SetBool("IsWalking", true);
            StartCoroutine(MoveThroughPath(path));
            pathfinder.ResetPathfinder();      
        }
    }

    public bool CanReachTile(Tile tile)
    {
        return tile.CanBeReached && !tile.Occupied;
    }

    public void FindPaths(Tile target)
    {
        pathfinder.FindPaths(target, moveData.MaxMove);
    }

    void FinalizePosition(Tile tile)
    {
        transform.position = tile.transform.position;
        Location = tile;
        Moving = false;
        tile.Occupied = true;
        tile.occupyingCharacter = this;
    }

    void MoveAndRotate(Vector3 origin, Vector3 destination, float duration)
    {
        transform.position = Vector3.Lerp(origin, destination, duration);
        Vector3 vectorRotation = origin.DirectionTo(destination).Flat();

        if (vectorRotation != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(vectorRotation, Vector3.up);
    }

}
