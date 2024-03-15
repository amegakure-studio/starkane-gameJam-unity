using System;
using System.Collections.Generic;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Dojo;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Amegakure.Starkane.Config
{
    public class WorldPlayerCreator : MonoBehaviour
    {
        private WorldManager worldManager;
        private Dictionary<CharacterType, string> characterPrefabsDict;
        private bool hasBeenCreated = false;

        private void Awake()
        {
            characterPrefabsDict =  new();
            characterPrefabsDict[CharacterType.Archer] = "Darkelf";
            characterPrefabsDict[CharacterType.Cleric] = "Wizard";
            characterPrefabsDict[CharacterType.Warrior] = "Avelyn";
            characterPrefabsDict[CharacterType.Goblin] = "Goblin";

            Debug.LogWarning("Load in: " + SceneManager.GetActiveScene().name);

            try {
                Session session = GameObject.FindObjectOfType<Session>();
                //if (session != null)
                //    Debug.Log("AWAKE: " + session.Player);
                //else
                //    Debug.Log("Null session ");
            }
            catch { Debug.LogError("ERROR"); }
            

        }

        void OnEnable()
        {
            worldManager = GameObject.FindAnyObjectByType<WorldManager>();
            worldManager.OnEntityFeched += Create;   
        }

        void OnDisable()
        {
            worldManager.OnEntityFeched -= Create;   
        }

        private void Create(WorldManager worldManager)
        {
            if (hasBeenCreated)
                return;

            //Debug.Log("Create called!!");

            Session session = GameObject.FindObjectOfType<Session>();
            //if (session != null)
            //    Debug.Log("Session load? " + session.Player);
            //else
            //    Debug.Log("Null session ");

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

                        if(characterPlayerProgress.owner.Hex().Equals(player.HexID)
                            && characterType == player.DefaultCharacter)
                        {
                            player.SetDojoId(characterPlayerProgress.Owner);

                            
                            Entities.Character characterEntity = GetCharacter((int)characterType);

                            GameObject characterGo = builder
                                    .AddCharacterPrefab(characterType, characterPrefabsDict[characterType], characterPlayerProgress, characterEntity)
                                    .AddGridMovement(PathStyle.SQUARE, 50)
                                    .AddCharacterController()
                                    .Build();
                            
                            hasBeenCreated = true;
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
                        int characterId = checked((int)characterEntity.character_id);

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
