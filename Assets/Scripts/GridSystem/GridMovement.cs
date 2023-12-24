using Amegakure.Starkane.PubSub;
using System.Collections;
using UnityEngine;
using Amegakure.Starkane.Entities;

namespace Amegakure.Starkane.GridSystem
{
    public class GridMovement : MonoBehaviour
    {
        #region member fields

        [SerializeField] PathStyle tileStyle;
        [SerializeField] int numOfTiles;
        [SerializeField] float speed;

        private Pathfinder m_Pathfinder;

        public bool Moving { get; private set; } = false; 
        
        #endregion

        private void Awake()
        {
            m_Pathfinder = new();

            //if (Location != null)
            //    FinalizePosition(Location);
        }


        IEnumerator MoveThroughPath(Path path, Character character)
        {
            EventManager.Instance.Publish(GameEvent.CHARACTER_MOVE_START, new() { { "Character", character } });

            int step = 0;
            int pathlength = Mathf.Clamp(path.TilesInPath.Length, 0, numOfTiles + 1);
            Tile currentTile = path.TilesInPath[0];
            float animationtime = 0f;
            const float minimumistanceFromNextTile = 0.05f;

            while (step < pathlength)
            {
                yield return null;
                Vector3 nextTilePosition = path.TilesInPath[step].transform.position;

                MoveAndRotate(currentTile.transform.position, nextTilePosition, animationtime / speed);
                animationtime += Time.deltaTime;

                if (Vector3.Distance(transform.position, nextTilePosition) > minimumistanceFromNextTile)
                    continue;

                currentTile = path.TilesInPath[step];
                step++;
                animationtime = 0f;
            }

            FinalizePosition(path.TilesInPath[pathlength - 1], character);

            EventManager.Instance.Publish(GameEvent.CHARACTER_MOVE_END, new() { { "Character", character } });

        }

        public bool GoTo(Tile origin, Character character, Tile target)
        {
            if (CanReachTile(target))
            {
                Path path = m_Pathfinder.PathBetween(target, origin);

                Moving = true;
                origin.OccupyingObject = null;               
                StartCoroutine(MoveThroughPath(path, character));
                ClearMovementFrontier();

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
                Frontier frontier = m_Pathfinder.FindPaths(target, numOfTiles, tileStyle, true);
                return frontier;
            }

            else
                return new Frontier();
        }

        private void FinalizePosition(Tile tile, Character character)
        {
            transform.position = tile.transform.position;
            //Location = tile;
            Moving = false;
            tile.OccupyingObject = character.gameObject;
        }

        private void MoveAndRotate(Vector3 origin, Vector3 destination, float duration)
        {
            transform.position = Vector3.Lerp(origin, destination, duration);
            Vector3 vectorRotation = origin.DirectionTo(destination).Flat();

            if (vectorRotation != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(vectorRotation, Vector3.up);
        }
    }
}
