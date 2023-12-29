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
        private Entities.Character characterEntity;
        private CharacterState characterState;
        private ActionState actionState;

        private bool isMoving = false;
        private GridManager gridManager;

        public CharacterPlayerProgress CharacterPlayerProgress { get => characterPlayerProgress; set => characterPlayerProgress = value; }
        public Entities.Character CharacterEntity { get => characterEntity; set => characterEntity = value; }
        public GridMovement GridMovement 
        {
            get => gridMovement; 
            set 
            {
                if(gridMovement != null)
                {
                    gridMovement.OnMovementStart -= OnMovementStart;
                    gridMovement.OnMovementEnd -= OnMovementEnd;
                }
                gridMovement = value;
                
                gridMovement.OnMovementStart += OnMovementStart;
                gridMovement.OnMovementEnd += OnMovementEnd;

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
            gridMovement.OnMovementStart -= OnMovementStart;
            gridMovement.OnMovementEnd -= OnMovementEnd;
        }

        private void OnMovementStart(Tile tile)
        {
            isMoving = true;
            EventManager.Instance.Publish(GameEvent.CHARACTER_MOVE_START, new() { { "Character", this } });
        }

        private void OnMovementEnd(Tile tile)
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

        public int GetHp()
        {
            return (int) characterState.Remain_hp;
        }

        public int GetMp()
        {
            return (int)characterState.Remain_mp;
        }

        public int GetMovementRange()
        {
            return characterEntity.Movement_range;
        }

        public float GetHpNormalized()
        {
            return (float) GetHp() / (float) characterEntity.Hp;
        }

        public float GetMpNormalized()
        {
            return (float) GetMp() / (float)characterEntity.Hp;
        }

        public string GetCharacterName()
        {
            return gameObject.name;
        }
    }
}

