using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using bottlenoselabs.C2CS.Runtime;
using Dojo.Starknet;
using dojo_bindings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Character = Amegakure.Starkane.EntitiesWrapper.Character;

public class Combat : MonoBehaviour
{
    private MatchState matchState;
    private Dictionary<Player, List<Character>> playerMatchCharacters = new();
    private Dictionary<Player, List<ActionState>> actionStates = new();
    private Dictionary<Player, List<CharacterState>> characterStates = new();
    private DojoTxConfig dojoTxConfig;
    private GridManager gridManager;

    public MatchState MatchState
    {
        get => matchState;
        set
        {
            matchState = value;
            matchState.playerTurnIdChanged += MatchState_playerTurnIdChanged;
            matchState.winnerChanged += MatchState_winnerChanged;
        }
    }

    public Dictionary<Player, List<Character>> PlayerMatchCharacter { get => playerMatchCharacters; private set => playerMatchCharacters = value; }
    public Dictionary<Player, List<ActionState>> ActionStates { get => actionStates; private set => actionStates = value; }
    public Dictionary<Player, List<CharacterState>> CharacterStates { get => characterStates; private set => characterStates = value; }

    private void Awake()
    {
        playerMatchCharacters = new();
        actionStates = new();
        characterStates = new();
        dojoTxConfig = GameObject.FindAnyObjectByType<DojoTxConfig>();
        gridManager = GameObject.FindAnyObjectByType<GridManager>();
    }

    private void Start()
    {
        string playerId = matchState.player_turn.Hex();
        // Debug.Log("Match turn playerID:" + playerId);
        // foreach (Player player in playerMatchCharacters.Keys)
        // {
        //     Debug.Log("ID in dict: " + player.Id);
        // }
        Player playerTurn = playerMatchCharacters.Keys.First(p => p.HexID.Equals(playerId));
        
        Debug.Log("playerTurn:" + playerTurn.HexID);

        EventManager.Instance.Publish(GameEvent.COMBAT_TURN_CHANGED, new() { { "Player", playerTurn } });
    }


    private void OnDisable()
    {
        matchState.playerTurnIdChanged -= MatchState_playerTurnIdChanged;
        matchState.winnerChanged -= MatchState_winnerChanged;
    }

    private void MatchState_playerTurnIdChanged(string playerId)
    {
        Player playerTurn = playerMatchCharacters.Keys.First(p => p.HexID.Equals(playerId));

        EventManager.Instance.Publish(GameEvent.COMBAT_TURN_CHANGED, new() { { "Player", playerTurn } });
    }


    private void MatchState_winnerChanged(string playerId)
    {
        Player playerWinner = playerMatchCharacters.Keys.First(p => p.HexID.Equals(playerId));
        Debug.Log("!!!!The Winnerr ....... is: " + playerWinner.PlayerName);

        EventManager.Instance.Publish(GameEvent.COMBAT_VICTORY, new() { { "Player", playerWinner } });
    }


    public void AddCharacter(Player player, Character character, ActionState actionState, CharacterState characterState)
    {
        if (!playerMatchCharacters.ContainsKey(player))
            playerMatchCharacters[player] = new();

        List<Character> currentCharacters = playerMatchCharacters[player];
        currentCharacters.Add(character);

        if (!actionStates.ContainsKey(player))
            actionStates[player] = new();

        List<ActionState> currentActionStates = actionStates[player];
        currentActionStates.Add(actionState);

        if (!characterStates.ContainsKey(player))
            characterStates[player] = new();

        List<CharacterState> currentCharacterStates = characterStates[player];
        currentCharacterStates.Add(characterState);
        characterState.OnDead += HandleOnDead;
        characterState.OnPositionChange += HandleOnPositionChange;

        if (!character.IsAlive())
            EventManager.Instance.Publish(GameEvent.COMBAT_CHARACTER_DEAD, new() { { "Character", character } });
        // character.ResetLocation();
    }

    private void HandleOnPositionChange(CharacterState state)
    {
        Debug.Log("Position changed x:" + (int)state.x + " y: " + (int)state.y);

        Player player = GetPlayerByID(state.player.Hex());
        if (player != null)
        {
            Debug.Log("Player on match");
            Character character = playerMatchCharacters[player].Find(pmc => pmc.CharacterState.GetInstanceID() == state.GetInstanceID()); ;
            if (character != null)
            {
                
                UnityEngine.Vector2Int tilePos = new UnityEngine.Vector2Int((int)state.x, (int)state.y);
                Debug.Log("Vecto2 :" + tilePos);
                
                Tile target = gridManager.WorldMap[tilePos];
                
                if (target != null)
                {
                    character.Move(target);
                }
                else
                    Debug.LogError("Tile not found");
            } 
        }
    }

    private void HandleOnDead(CharacterState state)
    {
        Player player = GetPlayerByID(state.player.Hex());
        if(player != null)
        {
            Character character = playerMatchCharacters[player].Find(pmc => pmc.CharacterState.GetInstanceID() == state.GetInstanceID());;
            if(character != null)
                EventManager.Instance.Publish(GameEvent.COMBAT_CHARACTER_DEAD, new() { { "Character", character } });
        }
    }

    public void Move(Character character, Player player, Tile target)
    {
        if (CanMove(character, player))
        {
            CallMoveTx(character, player, target);
        }
    }

    private async void CallMoveTx(Character character, Player player, Tile target)
    {
        // Debug.Log("Move: " + player.PlayerName + "With: " + character.CharacterName
        //             + "To: " + target.coordinate.ToString());
        
        
        var provider = new JsonRpcClient(dojoTxConfig.RpcUrl);
        var account = new Account(provider, dojoTxConfig.GetKatanaPrivateKey(), new FieldElement(dojoTxConfig.KatanaAccounAddress));

        string match_id_string = matchState.Id.ToString("X");
        var match_id = new FieldElement(match_id_string).Inner();

        var player_id = new FieldElement(player.HexID).Inner();

        string character_id_string = character.GetId().ToString("X");
        var character_id = new FieldElement(character_id_string).Inner();

        string x_string = target.coordinate.x.ToString("X");
        var x = new FieldElement(x_string).Inner();

        string y_string = target.coordinate.y.ToString("X");
        var y = new FieldElement(y_string).Inner();

        dojo.Call call = new()
        {
            calldata = new[]
            {
                   match_id, player_id, character_id, x, y
                },
            to = dojoTxConfig.MoveSystemActionAddress,
            selector = "move"
        };

        try
        {
            await account.ExecuteRaw(new[] { call });
            //character.Move(target);
            //Debug.Log("Move ended!");
        }
        catch 
        {
            Debug.LogError("Error in move tx");
        }
        
    }

    public async void CallEndTurnTX(Player player)
    {
        //TODO: check if player == playerTurn
        var provider = new JsonRpcClient(dojoTxConfig.RpcUrl);
        var account = new Account(provider, dojoTxConfig.GetKatanaPrivateKey(), new FieldElement(dojoTxConfig.KatanaAccounAddress));

        List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();

        string match_id_string = matchState.Id.ToString("X");
        var match_id = new FieldElement(match_id_string).Inner();

        var player_id = new FieldElement(player.HexID).Inner();

        dojo.Call call = new ()
        {
            calldata = new[]
            {
                   match_id, player_id
                },
            to = dojoTxConfig.TurnSystemActionAddress,
            selector = "end_turn"
        };

        await account.ExecuteRaw(new[] { call });
    }

    public bool CanMove(Character character, Player player)
    {
        bool playerRegistered = playerMatchCharacters.ContainsKey(player) && playerMatchCharacters[player].Contains(character);

        if (playerRegistered)
        {
            bool characterMoved = GetActionState(player, character).Movement;
            return matchState.player_turn.Hex() == player.HexID && !characterMoved;
        }

        return false;
    }

    private ActionState GetActionState(Player player, Character character)
    {
        return actionStates[player].Find(action => (int)action.Character_id == character.GetId());
    }

    public List<Character> GetCharacters(Player player)
    {
        return playerMatchCharacters[player];
    }

    public Player GetActualTurnPlayer()
    {
        foreach (Player player in playerMatchCharacters.Keys)
        {
            if (player.HexID.Equals(matchState.player_turn.Hex()))
            {
                return player;
            }
        }

        return null;
    }

    public Player GetPlayerByID(string id)
    {
        foreach (Player player in playerMatchCharacters.Keys)
        {
            if (player.HexID.Equals(id))
            {
                return player;
            }
        }

        return null;
    }

    public void DoSkill(Player playerFrom, Character characterFrom,
                            Skill skill, Player playerReceiver, Character characterReceiver)
    {
        if (CanDoSkill(playerFrom, characterFrom, skill) && characterReceiver.IsAlive())
        {
            CallSkillTX(playerFrom, characterFrom, skill, playerReceiver, characterReceiver);

            if (!characterReceiver.IsAlive())
                EventManager.Instance.Publish(GameEvent.COMBAT_CHARACTER_DEAD, new() { { "Character", characterReceiver } });
        }
    }

    private async void CallSkillTX(Player playerFrom, Character characterFrom,
                            Skill skill, Player playerReceiver, Character characterReceiver)
    {
        var provider = new JsonRpcClient(dojoTxConfig.RpcUrl);
        var account = new Account(provider, dojoTxConfig.GetKatanaPrivateKey(), new FieldElement(dojoTxConfig.KatanaAccounAddress));

        List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();

        string match_id_string = matchState.Id.ToString("X");
        var match_id = new FieldElement(match_id_string).Inner();

        var player_id_from = new FieldElement(playerFrom.HexID).Inner();

        string character_id_from_string = characterFrom.GetId().ToString("X");
        var character_id_from = new FieldElement(character_id_from_string).Inner();

        string skill_id_string = skill.Id.ToString("X");
        var skill_id = new FieldElement(skill_id_string).Inner();

        string level_string = skill.Level.ToString("X");
        var level = new FieldElement(level_string).Inner();

        var player_id_receiver = new FieldElement(playerReceiver.HexID).Inner();

        string character_id_receiver_string = characterReceiver.GetId().ToString("X");
        var character_id_receiver = new FieldElement(character_id_receiver_string).Inner();

        dojo.Call call = new()
        {
            calldata = new[]
            {
                   match_id, player_id_from, character_id_from, skill_id, level,
                   player_id_receiver, character_id_receiver
                },
            to = dojoTxConfig.ActionSystemActionAddress,
            selector = "action"
        };

        try
        {
            await account.ExecuteRaw(new[] { call });

            Dictionary<string, object> context = new()
            {
                { "PlayerFrom", playerFrom },
                { "CharacterFrom", characterFrom },
                { "Skill", skill },
                { "PlayerReceiver", playerReceiver },
                { "CharacterReceiver", characterReceiver }
            };

            EventManager.Instance.Publish(GameEvent.COMBAT_SKILL_DONE, context);
        }
        catch 
        {
            Debug.LogError("Error in do skill tx");
        }
        
    }

    public bool CanDoSkill(Player player, Character character, Skill skillSelected)
    {
        bool playerRegistered = playerMatchCharacters.ContainsKey(player) && playerMatchCharacters[player].Contains(character);

        if (playerRegistered)
        {
            // Debug.Log("Player name: " + player.PlayerName);
            ActionState playerActionState = GetActionState(player, character);
            CharacterState playerCharacterState = GetCharacterState(player, character);
            // Debug.Log("Character using: " + character.CharacterName);

            bool skillPerformed = playerActionState.Action;
            bool enoughMana = playerCharacterState.Remain_mp >= skillSelected.Mp_cost;
            // Debug.Log(" skillPerformed " + skillPerformed);
            // Debug.Log("enoughMana " + enoughMana);

            return !skillPerformed && enoughMana;
        }

        return false;
    }

    private CharacterState GetCharacterState(Player player, Character character)
    {
        return characterStates[player].Find(characterState => (int)characterState.Character_id == character.GetId());
    }

    internal List<Character> GetRivalCharacters(Player player)
    {
        // TODO: Verify if p != player works correctly
        List<Player> rivals = playerMatchCharacters.Keys.Where(p => p != player).ToList();
        List<Character> rivalCharacters = new();

        rivals.ForEach(r => rivalCharacters.AddRange(GetCharacters(r)));

        return rivalCharacters;
    }
}
