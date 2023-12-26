using System.Collections;
using UnityEngine;
using System;

namespace Amegakure.Starkane.GridSystem
{
    public class GridMovement : MonoBehaviour
    {
        #region member fields

        [SerializeField] PathStyle tileStyle;
        [SerializeField] int numOfTiles;
        [SerializeField] float speed;
        private Pathfinder m_Pathfinder;
        private bool corroutineStarted = false;
        public event Action<Tile> OnMovementStart;
        public event Action<Tile> OnMovementEnd;

        public PathStyle TileStyle { get => tileStyle; set => tileStyle = value; }
        public int NumOfTiles { get => numOfTiles; set => numOfTiles = value; }
        public float Speed { get => speed; set => speed = value; }

        #endregion

        private void Awake()
        {
            m_Pathfinder = new();
        }

        IEnumerator MoveThroughPath(Path path, Tile origin, Tile target)
        {
            int step = 0;
            int pathlength = Mathf.Clamp(path.TilesInPath.Length, 0, NumOfTiles + 1);
            Tile currentTile = path.TilesInPath[0];
            float animationtime = 0f;
            const float minimumistanceFromNextTile = 0.05f;

            while (step < pathlength)
            {
                yield return null;
                Vector3 nextTilePosition = path.TilesInPath[step].transform.position;

                MoveAndRotate(currentTile.transform.position, nextTilePosition, animationtime * Speed);
                animationtime += Time.deltaTime;

                if (Vector3.Distance(transform.position, nextTilePosition) > minimumistanceFromNextTile)
                    continue;

                currentTile = path.TilesInPath[step];
                step++;
                animationtime = 0f;
            }

            // FinalizePosition(path.TilesInPath[pathlength - 1]);
            OnMovementEnd?.Invoke(target);
            //     transform.position = tile.transform.position;
            corroutineStarted = false;
        }

        public bool GoTo(Tile origin, Tile target)
        {
            if (CanReachTile(target))
            {
                OnMovementStart?.Invoke(origin);
                Path path = m_Pathfinder.PathBetween(target, origin);

                origin.OccupyingObject = null;
                if(!corroutineStarted)
                {
                    corroutineStarted = true;
                    StartCoroutine(MoveThroughPath(path, origin, target));
                    ClearMovementFrontier();
                }               
    
                return true;
            }

            else return false;
        }

        private void ClearMovementFrontier()
        {
            m_Pathfinder.ResetPathfinder();
        }

        public bool CanReachTile(Tile tile)
        {
            return tile.CanBeReached && !tile.Occupied() && m_Pathfinder.IsTileInFrontier(tile);
        }

        public Frontier FindPaths(Tile target)
        {
            if (target != null)
            {
                Frontier frontier = m_Pathfinder.FindPaths(target, NumOfTiles, TileStyle, true);
                return frontier;
            }

            else
                return new Frontier();
        }

        // private void FinalizePosition(Tile tile)
        // {
        //     OnMovementEnd?.Invoke(this, EventArgs.Empty);
        //     transform.position = tile.transform.position;
        //     Moving = false;
        // }

        private void MoveAndRotate(Vector3 origin, Vector3 destination, float duration)
        {
            transform.position = Vector3.Lerp(origin, destination, duration);
            Vector3 vectorRotation = origin.DirectionTo(destination).Flat();

            if (vectorRotation != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(vectorRotation, Vector3.up);
        }
    }
}
