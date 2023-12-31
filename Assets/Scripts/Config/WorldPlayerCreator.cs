
using System;
using System.Collections.Generic;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Dojo;
using UnityEngine;

namespace Amegakure.Starkane.Config
{
    public class WorldPlayerCreator : MonoBehaviour
    {
        [SerializeField] WorldManager worldManager;
        private Dictionary<CharacterType, string> characterPrefabsDict;

        private void Awake()
        {
            characterPrefabsDict =  new();
            characterPrefabsDict[CharacterType.Archer] = "Darkelf";
            characterPrefabsDict[CharacterType.Cleric] = "Wizard";
            characterPrefabsDict[CharacterType.Warrior] = "Avelyn";
            characterPrefabsDict[CharacterType.Goblin] = "Goblin";
        }

        void OnEnable()
        {
            worldManager.OnEntityFeched += Create;   
        }

        void OnDisable()
        {
            worldManager.OnEntityFeched -= Create;   
        }

        private void Create(WorldManager worldManager)
        {
            Session session = GameObject.FindObjectOfType<Session>();
            Player player  = session.Player;
        
            GameObject builderGo = Instantiate(new GameObject());
            CharacterBuilder builder = builderGo.AddComponent<CharacterBuilder>();
    
            if (player != null)
            {
                GameObject[] entities = worldManager.Entities();

                foreach(GameObject go in entities)
                {
                    CharacterPlayerProgress characterPlayerProgress = go.GetComponent<CharacterPlayerProgress>();
                
                    if (characterPlayerProgress != null )
                    {
                        CharacterType characterType = characterPlayerProgress.GetCharacterType();

                        if(characterPlayerProgress.getPlayerID() == player.Id
                            && characterType == player.DefaultCharacter)
                        {
                            player.SetDojoId(characterPlayerProgress.Owner);

                            Entities.Character characterEntity = GetCharacter((int)characterType);

                            GameObject characterGo = builder
                                    .AddCharacterPrefab(characterType, characterPrefabsDict[characterType], characterPlayerProgress, characterEntity)
                                    .AddGridMovement(PathStyle.SQUARE, 50)
                                    .AddCharacterController()
                                    .Build();

                            // characterGo.transform.parent = player.gameObject.transform;
                        }
                    }
                }

            }
        }

        private Entities.Character GetCharacter(int _characterId)
        {
            GameObject[] entities = worldManager.Entities();

            foreach (GameObject go in entities)
            {
                try
                {
                    Entities.Character characterEntity = go.GetComponent<Entities.Character>();

                    if (characterEntity != null)
                    {
                        int characterId = checked((int)characterEntity.Character_id);

                        if (characterId == (_characterId))
                            return characterEntity;
                    }
                }
                catch { }
            }

            throw new ArgumentException("Couldn't get character entity");
        }
    }
}
