using Amegakure.Starkane.PubSub;
using System.Collections;
using UnityEngine;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.Id;

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
        public PathStyle TileStyle { get => tileStyle; set => tileStyle = value; }
        public int NumOfTiles { get => numOfTiles; set => numOfTiles = value; }
        public float Speed { get => speed; set => speed = value; }

        #endregion

        private void Awake()
        {
            m_Pathfinder = new();

            //if (Location != null)
            //    FinalizePosition(Location);
        }


        IEnumerator MoveThroughPath(Path path, CharacterId characterId)
        {
            EventManager.Instance.Publish(GameEvent.CHARACTER_MOVE_START, new() { { "Character", characterId } });

            int step = 0;
            int pathlength = Mathf.Clamp(path.TilesInPath.Length, 0, NumOfTiles + 1);
            Tile currentTile = path.TilesInPath[0];
            float animationtime = 0f;
            const float minimumistanceFromNextTile = 0.05f;

            while (step < pathlength)
            {
                yield return null;
                Vector3 nextTilePosition = path.TilesInPath[step].transform.position;

                MoveAndRotate(currentTile.transform.position, nextTilePosition, animationtime / Speed);
                animationtime += Time.deltaTime;

                if (Vector3.Distance(transform.position, nextTilePosition) > minimumistanceFromNextTile)
                    continue;

                currentTile = path.TilesInPath[step];
                step++;
                animationtime = 0f;
            }

            FinalizePosition(path.TilesInPath[pathlength - 1], characterId);

            EventManager.Instance.Publish(GameEvent.CHARACTER_MOVE_END, new() { { "Character", characterId } });

        }

        public bool GoTo(Tile origin, CharacterId character, Tile target)
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
                Frontier frontier = m_Pathfinder.FindPaths(target, NumOfTiles, TileStyle, true);
                return frontier;
            }

            else
                return new Frontier();
        }

        private void FinalizePosition(Tile tile, CharacterId characterId)
        {
            transform.position = tile.transform.position;
            //Location = tile;
            Moving = false;
            tile.OccupyingObject = characterId.CharacterGo;
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
