using System;
using System.Collections.Generic;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Dojo;
using UnityEngine;

public class CombatPlayerCreator : MonoBehaviour
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
        int match_id = GetMatchID();

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

                    if(characterPlayerProgress.getID() == player.Id)
                    {
                        player.SetDojoId(characterPlayerProgress.Owner);

                        CharacterState characterState = GetCharacterState(player.Id, match_id, (int)characterType );
                        ActionState actionState = GetCharacterActionState(player.Id, match_id, (int)characterType );

                        GameObject characterGo = builder
                                .AddCharacterPrefab(characterType, characterPrefabsDict[characterType], characterPlayerProgress)
                                .AddCombatElements(characterState, actionState)
                                .AddGridMovement()
                                .AddCharacterController()
                                .Build();

                        characterGo.transform.parent = player.gameObject.transform;
                    }
                    // else is the adversary!!!
                }
            }

        }
    }

    private int GetMatchID()
    {
        GameObject[] entities = worldManager.Entities();

        foreach(GameObject go in entities)
        {
            try
            {
                MatchState matchState = go.GetComponent<MatchState>();
                if(matchState != null)
                {
                    if(matchState.WinnerId != 0)
                    {
                        return checked((int)matchState.Id);
                    }
                }
            }
            catch{}
        }

        return 0;
    }

    private CharacterState GetCharacterState(int playerID, int matchID, int characterID)
    {
        GameObject[] entities = worldManager.Entities();

        foreach(GameObject go in entities)
        {
            try
            {
                CharacterState characterState = go.GetComponent<CharacterState>();
                if(characterState != null)
                {
                    int match_id = checked((int)characterState.Match_id);
                    int character_id = checked((int)characterState.Character_id);
                    int player_id = characterState.Player_id;

                    // Debug.Log("Component data: \n match_id"+ match_id
                    //         + "\n character_id" + character_id
                    //         + "\n player_id" + player_id);

                    // Debug.Log("Unity data: \n match_id"+ matchID
                    //         + "\n character_id" + characterID
                    //         + "\n player_id" + playerID);


                    if(match_id == matchID && player_id == playerID
                    && character_id == characterID - 1)
                        return characterState;
                }
            }
            catch{}
        }

        throw new ArgumentException("Couldn't get character state");
    }

    private ActionState GetCharacterActionState(int playerID, int matchID, int characterID)
    {
        GameObject[] entities = worldManager.Entities();

        foreach(GameObject go in entities)
        {
            try
            {
                ActionState actionState = go.GetComponent<ActionState>();
                
                if(actionState != null)
                {
                    int match_id = checked((int)actionState.Match_id);
                    int character_id = checked((int)actionState.Character_id);
                    int player_id = actionState.Player_id;

                            Debug.Log("Component data: \n match_id"+ match_id
                            + "\n character_id" + character_id
                            + "\n player_id" + player_id);

                    Debug.Log("Unity data: \n match_id"+ matchID
                            + "\n character_id" + characterID
                            + "\n player_id" + playerID);
                    
                    bool matchRes = match_id == matchID;
                    bool playerRes = player_id == playerID;
                    bool characterRes = character_id == (characterID - 1);

                    Debug.Log("matchRes" + matchRes
                            + "\n playerRes" + playerRes
                            + "\n characterRes" + characterRes );

                    if( matchRes && playerRes   && characterRes)
                        return actionState;
                }
            }
            catch{}
        }

        throw new ArgumentException("Couldn't get character state");
    }
}
