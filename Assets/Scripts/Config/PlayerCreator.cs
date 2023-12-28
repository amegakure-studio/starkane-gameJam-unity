
using System.Collections.Generic;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Dojo;
using UnityEngine;

namespace Amegakure.Starkane.Config
{
    public class PlayerCreator : MonoBehaviour
    {
        [SerializeField] WorldManager worldManager;
        private Dictionary<CharacterType, string> characterPrefabsDict;

        private void Awake()
        {
            characterPrefabsDict =  new();
            characterPrefabsDict[CharacterType.Warrior] = "Avelyn";
            characterPrefabsDict[CharacterType.Pig] = "Enemy";
            characterPrefabsDict[CharacterType.Archer] = "Darkelf";
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
            Player player  = GameObject.FindObjectOfType<Player>();
        
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

                        if(characterPlayerProgress.getID() == player.Id
                            && characterType == player.DefaultCharacter)
                        {
                            player.SetDojoId(characterPlayerProgress.Owner);
                        
                            GameObject characterGo = builder
                                    .AddCharacterPrefab(characterType, characterPrefabsDict[characterType], characterPlayerProgress)
                                    .AddGridMovement()
                                    .AddCharacterController()
                                    .Build();

                            characterGo.transform.parent = player.gameObject.transform;
                        }
                    }
                }

            }


        
        }
    }
}
