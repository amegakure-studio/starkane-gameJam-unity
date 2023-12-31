using System;
using System.Collections.Generic;
using System.Numerics;
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
            characterPrefabsDict[CharacterType.Archer] = "Darkelf";
            characterPrefabsDict[CharacterType.Cleric] = "Wizard";
            characterPrefabsDict[CharacterType.Warrior] = "Avelyn";
            characterPrefabsDict[CharacterType.Goblin] = "Goblin";
        }

        private void Start()
        {
            
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
            Player adversary = null;

            MatchState matchState = this.GetMatchState();
            int match_id = checked((int)matchState.Id);

            combat.MatchState = matchState;

            List<BigInteger> matchPlayers = FindMatchPlayers(match_id);
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
                        List<int> matchPlayerCharacterID =  FindPlayerCharactersInAMatch(match_id, player.Id);
                        CharacterType characterType = characterPlayerProgress.GetCharacterType();

                        if(characterPlayerProgress.getPlayerID().Equals(player.Id)
                        && matchPlayerCharacterID.Contains((int)characterType))
                        {
                            Debug.Log("ID Player" + player.Id);
                            Debug.Log("Dojo Player" + characterPlayerProgress.getPlayerID());

                            CharacterState characterState = GetCharacterState(player.Id, match_id, (int)characterType );
                            ActionState actionState = GetCharacterActionState(player.Id, match_id, (int)characterType );
                            List<Skill> characterSkills = GetSkills((int)characterType);
                            Amegakure.Starkane.Entities.Character characterEntity = GetCharacter((int)characterType);

                            GameObject characterGo = builder
                                    .AddCharacterPrefab(characterType, characterPrefabsDict[characterType],
                                                        characterPlayerProgress, characterEntity)
                                    .AddCombatElements(characterState, actionState, characterSkills)
                                    .AddGridMovement(PathStyle.SQUARE, characterEntity.Movement_range)
                                    .AddCombatCharacterController(player, combat)
                                    .Build();
                            
                            Character character = characterGo.GetComponent<Character>();
                            combat.AddCharacter(player, character, actionState, characterState);
                            Debug.Log("Added: " + player.PlayerName + "Character: " + character.CharacterName );

                            // characterGo.transform.parent = player.gameObject.transform;
                        }
                        else 
                        {
                            BigInteger adversaryID = characterPlayerProgress.getPlayerID();
                            List<int> matchAdversaryCharacterID =  FindPlayerCharactersInAMatch(match_id, adversaryID);

                            if (matchPlayers.Contains(adversaryID) && 
                                matchAdversaryCharacterID.Contains((int)characterType))
                            {
                                Debug.Log("ID ADV:" + adversaryID);
                                CharacterState characterState = GetCharacterState(adversaryID, match_id, (int)characterType);
                                ActionState actionState = GetCharacterActionState(adversaryID, match_id, (int)characterType);
                                List<Skill> characterSkills = GetSkills((int)characterType);
                                Entities.Character characterEntity = GetCharacter((int)characterType);

                                GameObject characterGo = builder
                                        .AddCharacterPrefab(characterType, characterPrefabsDict[characterType], 
                                                            characterPlayerProgress, characterEntity)
                                        .AddCombatElements(characterState, actionState, characterSkills)
                                        .AddGridMovement(PathStyle.SQUARE, characterEntity.Movement_range)
                                        .Build();

                                if (adversary == null)
                                    adversary = CreateAdversary(adversaryID);
                            

                                Character character = characterGo.GetComponent<Character>();
                                combat.AddCharacter(adversary, character, actionState, characterState);

                                characterGo.transform.parent = adversary.transform;
                            }                       
                        }
                    }
                }

            }

            Destroy(builderGo);
        }

        private Player CreateAdversary(BigInteger adversaryId)
        {
            GameObject adversaryGo = Instantiate(new GameObject());
            Player adversary = adversaryGo.AddComponent<Player>();
            adversary.Id = adversaryId;
            adversary.name = "Enemy";
            adversary.PlayerName = adversary.name;
            
            PlayerAIController aiController = adversaryGo.AddComponent<PlayerAIController>();
            aiController.Player = adversary;

            return adversary;
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

        private List<BigInteger> FindMatchPlayers(BigInteger match_id)
        {
            List<BigInteger> matchPlayers = new();

            GameObject[] entities = worldManager.Entities();

            foreach (GameObject go in entities)
            {
                try
                {
                    MatchPlayer matchPlayer = go.GetComponent<MatchPlayer>();
                    if (matchPlayer != null)
                    {
                        if ((BigInteger) matchPlayer.Match_id == match_id)
                            matchPlayers.Add(matchPlayer.PlayerId);
                    }
                }
                catch { }
            }

            return matchPlayers;
        }

        private List<int> FindPlayerCharactersInAMatch(int match_id, BigInteger player_id)
        {
            List<int> playerCharactersInMatch = new();

            GameObject[] entities = worldManager.Entities();

            foreach (GameObject go in entities)
            {
                try
                {
                    MatchPlayerCharacter matchPlayerCharacter = go.GetComponent<MatchPlayerCharacter>();
                    if (matchPlayerCharacter != null)
                    {
                        if ((int) matchPlayerCharacter.Match_id == match_id
                            && matchPlayerCharacter.PlayerId.Equals(player_id))
                            playerCharactersInMatch.Add((int)matchPlayerCharacter.Character_id);
                    }
                }
                catch { }
            }

            return playerCharactersInMatch;
        }

        private List<Skill> GetSkills(int characterID)
        {
            List<Skill> skills = new();
            
            GameObject[] entities = worldManager.Entities();
            foreach(GameObject go in entities)
            { 
                try
                {
                    Skill skill = go.GetComponent<Skill>();
                    if(skill.Character_id == characterID)
                    {
                        // Debug.Log("Skill: " + skill.Id +
                        // " belongs to the character: " + characterID );
                        skills.Add(skill);
                    }
                }
                catch{}
            }

            return skills;
        }

        private MatchState GetMatchState()
        {
            GameObject[] entities = worldManager.Entities();

            foreach(GameObject go in entities)
            {
                try
                {
                    MatchState matchState = go.GetComponent<MatchState>();
                    Player player  = GameObject.FindObjectOfType<Session>().Player;
                    
                    if(matchState != null && player != null)
                    {
                        List<BigInteger> playerIdInMatch = FindMatchPlayers((BigInteger)matchState.Id);

                        if (matchState.WinnerId == 0 && playerIdInMatch.Contains(player.Id))
                        {
                            return matchState;
                        }
                    }
                }
                catch{}
            }

            return null;
        }

        private CharacterState GetCharacterState(BigInteger playerID, int matchID, int characterID)
        {
            // Debug.Log("!!!PID: " + playerID + " \n matchID" + matchID + "\n characterID" + characterID );
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
                        BigInteger player_id = characterState.Player_id;

                        // Debug.Log("Component data: \n match_id" + match_id
                        //         + "\n character_id" + character_id
                        //         + "\n player_id" + player_id);

                        // Debug.Log("Unity data: \n match_id" + matchID
                        //         + "\n character_id" + characterID
                        //         + "\n player_id" + playerID);


                        if (match_id == matchID && player_id.Equals(playerID)
                        && character_id == characterID)
                            return characterState;
                    }
                }
                catch{}
            }

            throw new ArgumentException("Couldn't get character state");
        }

        private ActionState GetCharacterActionState(BigInteger playerID, int matchID, int characterID)
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
                        BigInteger player_id = actionState.Player_id;

                        //         Debug.Log("Component data: \n match_id"+ match_id
                        //         + "\n character_id" + character_id
                        //         + "\n player_id" + player_id);

                        // Debug.Log("Unity data: \n match_id"+ matchID
                        //         + "\n character_id" + characterID
                        //         + "\n player_id" + playerID);
                    
                        bool matchRes = match_id == matchID;
                        bool playerRes = player_id.Equals(playerID);
                        bool characterRes = character_id == characterID;

                        // Debug.Log("matchRes" + matchRes
                        //         + "\n playerRes" + playerRes
                        //         + "\n characterRes" + characterRes );

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
