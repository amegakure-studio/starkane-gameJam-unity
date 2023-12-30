using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.Config
{
    public class CharacterBuilder : MonoBehaviour
    {
        [SerializeField] string charactersFolder = "Characters/";

        private GameObject characterGo;
        private Context context;

        private void Awake()
        {   
            context = GameObject.FindObjectOfType<Context>();
        }

        public CharacterBuilder AddCharacterPrefab(CharacterType characterType, string skinId, 
                                                    CharacterPlayerProgress characterPlayerProgress, 
                                                    Entities.Character characterEntity)
        {
            string folderCharacterType = characterType.ToString() + "/";
            string path = charactersFolder + folderCharacterType + skinId;
            
                try
                {
                    GameObject characterPrefab = Resources.Load<GameObject>(path);
                    characterGo = Instantiate(characterPrefab);
                
                    EntitiesWrapper.Character character = characterGo.AddComponent<EntitiesWrapper.Character>();
                
                    character.Location = context.GetInitialLocation();
                    character.CharacterPlayerProgress = characterPlayerProgress;
                    character.CharacterEntity = characterEntity;
                    character.CharacterName = skinId;

                    return this;
                }
                catch (Exception e) { Debug.LogError("Couldn't find characters folder: " + path + ". Details: " + e.Message); }      

            throw new ArgumentException("Couldn't create character");
        }

        public CharacterBuilder AddCombatElements( CharacterState characterState, ActionState actionState, List<Skill> skills)
        {
            try
            {
                Amegakure.Starkane.EntitiesWrapper.Character character
                                    = characterGo.GetComponent<Amegakure.Starkane.EntitiesWrapper.Character>();
        
                character.ActionState = actionState;
                character.CharacterState = characterState;
                character.Skills = skills;
                return this;
            }
            catch(Exception e){ Debug.LogError("Couldn't add elements to character" + e);}
        
            throw new ArgumentException("Couldn't add elements to character");
        }

        public CharacterBuilder AddCharacterController()
        {
            if (characterGo)
            {
                CharacterController controller = characterGo.AddComponent<CharacterController>();
                Amegakure.Starkane.EntitiesWrapper.Character character
                                    = characterGo.GetComponent<Amegakure.Starkane.EntitiesWrapper.Character>();
            
                controller.Character = character;
            }

            return this;
        }

        public CharacterBuilder AddCombatCharacterController(Player player, Combat combat)
        {
            if (characterGo)
            {
                CombatCharacterController controller = characterGo.AddComponent<CombatCharacterController>();
                Amegakure.Starkane.EntitiesWrapper.Character character
                                    = characterGo.GetComponent<Amegakure.Starkane.EntitiesWrapper.Character>();

                controller.Character = character;
                controller.Player = player;
                controller.Combat = combat;
            }

            return this;
        }

        public CharacterBuilder AddGridMovement(PathStyle pathStyle, int numOfTiles)
        {   
            if(characterGo)
            {
                GridMovement movement = characterGo.AddComponent<GridMovement>();
                movement.TileStyle = pathStyle;
                movement.NumOfTiles = numOfTiles;
                movement.Speed = 2;

                Amegakure.Starkane.EntitiesWrapper.Character character
                                    = characterGo.GetComponent<Amegakure.Starkane.EntitiesWrapper.Character>();
                character.GridMovement = movement;
            }

            return this;
        }

        public GameObject Build()
        {
            return characterGo;
        }

    }
}
