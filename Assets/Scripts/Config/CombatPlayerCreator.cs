using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Dojo;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Character = Amegakure.Starkane.EntitiesWrapper.Character;


namespace Amegakure.Starkane.Config
{
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
            GameObject combatGo = Instantiate(new GameObject());
            combatGo.name = "Combat";

            Combat combat = combatGo.AddComponent<Combat>();

            Player player  = GameObject.FindObjectOfType<Session>().Player;
            
            MatchState matchState = this.GetMatchState();
            int match_id = checked((int)matchState.Id);

            combat.MatchState = matchState;

            List<int> matchPlayers = FindMatchPlayers(match_id);
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
                            Amegakure.Starkane.Entities.Character characterEntity = GetCharacter((int)characterType);

                            GameObject characterGo = builder
                                    .AddCharacterPrefab(characterType, characterPrefabsDict[characterType], characterPlayerProgress, characterEntity)
                                    .AddCombatElements(characterState, actionState)
                                    .AddGridMovement(PathStyle.SQUARE, characterEntity.Movement_range)
                                    .AddCombatCharacterController(player, combat)
                                    .Build();
                            
                            Character character = characterGo.GetComponent<Character>();
                            combat.AddCharacter(player, character, actionState, characterState);

                            characterGo.transform.parent = player.gameObject.transform;
                        }
                        else 
                        {
                            int adversaryID = characterPlayerProgress.getID();

                            if (matchPlayers.Contains(adversaryID))
                            {
                                CharacterState characterState = GetCharacterState(adversaryID, match_id, (int)characterType);
                                ActionState actionState = GetCharacterActionState(adversaryID, match_id, (int)characterType);
                                Entities.Character characterEntity = GetCharacter((int)characterType);

                                GameObject characterGo = builder
                                        .AddCharacterPrefab(characterType, characterPrefabsDict[characterType], characterPlayerProgress, characterEntity)
                                        .AddCombatElements(characterState, actionState)
                                        .AddGridMovement(PathStyle.SQUARE, characterEntity.Movement_range)
                                        .Build();

                                GameObject adversaryGo = Instantiate(new GameObject());
                                Player adversary = adversaryGo.AddComponent<Player>();
                                adversary.Id = adversaryID;
                                adversary.name = "Enemy";
                                adversary.PlayerName = adversary.name;

                                Character character = characterGo.GetComponent<Character>();
                                combat.AddCharacter(adversary, character, actionState, characterState);

                                characterGo.transform.parent = adversaryGo.transform;
                            }                       
                        }
                    }
                }

            }

            Destroy(builderGo);
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

        private List<int> FindMatchPlayers(int match_id)
        {
            List<int> matchPlayers = new();

            GameObject[] entities = worldManager.Entities();

            foreach (GameObject go in entities)
            {
                try
                {
                    MatchPlayer matchPlayer = go.GetComponent<MatchPlayer>();
                    if (matchPlayer != null)
                    {
                        if ((int) matchPlayer.Match_id == match_id)
                            matchPlayers.Add(matchPlayer.PlayerId);
                    }
                }
                catch { }
            }

            return matchPlayers;
        }

        private MatchState GetMatchState()
        {
            GameObject[] entities = worldManager.Entities();

            foreach(GameObject go in entities)
            {
                try
                {
                    MatchState matchState = go.GetComponent<MatchState>();
                    if(matchState != null)
                    {
                        if (matchState.WinnerId == 0)
                        {
                            return matchState;
                        }
                    }
                }
                catch{}
            }

            return null;
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

                        Debug.Log("Component data: \n match_id" + match_id
                                + "\n character_id" + character_id
                                + "\n player_id" + player_id);

                        Debug.Log("Unity data: \n match_id" + matchID
                                + "\n character_id" + characterID
                                + "\n player_id" + playerID);


                        if (match_id == matchID && player_id == playerID
                        && character_id == (characterID))
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
                        bool characterRes = character_id == (characterID);

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
}
