using System;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using UnityEngine;

namespace Amegakure.Starkane.EntitiesWrapper
{
    public class Character : MonoBehaviour
    {
        private CharacterPlayerProgress characterPlayerProgress;
        private GridMovement gridMovement;
        private Tile location;
        private CharacterState characterState;
        private ActionState actionState;

        private bool isMoving = false;
        private GridManager gridManager;

        public CharacterPlayerProgress CharacterPlayerProgress { get => characterPlayerProgress; set => characterPlayerProgress = value; }
        public GridMovement GridMovement 
        {
            get => gridMovement; 
            set 
            {
                if(gridMovement != null)
                {
                    gridMovement.OnMovementStart -= onMovementStart;
                    gridMovement.OnMovementEnd -= onMovementEnd;
                }
                gridMovement = value;
                
                gridMovement.OnMovementStart += onMovementStart;
                gridMovement.OnMovementEnd += onMovementEnd;

                // if(characterState)
                // {    
                //     Vector2 characterPos = new(checked((int)characterState.X), checked((int)characterState.Y));
                //     this.Location = gridManager.WorldMap[characterPos];
                // }
            } 
        }

        private void Awake()
        {
            gridManager = GameObject.FindObjectOfType<GridManager>();
        }

        public Tile Location 
        {
            get => location;
            set 
            {
                if (location != null)
                    location.OccupyingObject = null;
                    
                location = value;
                location.OccupyingObject = gameObject;
                gameObject.transform.position = location.transform.position;
            } 
        }

        public CharacterState CharacterState 
        { 
            get => characterState;
            set
            {
                characterState = value;
                Vector2 characterPos = new(checked((int)characterState.X), checked((int)characterState.Y));
                this.Location = gridManager.WorldMap[characterPos];
            } 
        }
        public ActionState ActionState { get => actionState; set => actionState = value; }

        private void OnDisable()
        {
            gridMovement.OnMovementStart -= onMovementStart;
            gridMovement.OnMovementEnd -= onMovementEnd;
        }

        private void onMovementStart(Tile tile)
        {
            isMoving = true;
            EventManager.Instance.Publish(GameEvent.CHARACTER_MOVE_START, new() { { "Character", this } });
        }

        private void onMovementEnd(Tile tile)
        {
            Location = tile;
            isMoving = false;
            EventManager.Instance.Publish(GameEvent.CHARACTER_MOVE_END, new() { { "Character", this } });        
        }


        public void Move(Tile target)
        {
            if (!isMoving)
                gridMovement.GoTo(location, target);           
        }

        public Frontier GetMovementFrontier()
        {
            return gridMovement.FindPaths(location);
        }

        public int GetId()
        {
            return (int) characterPlayerProgress.GetCharacterType();
        }
    }
}

