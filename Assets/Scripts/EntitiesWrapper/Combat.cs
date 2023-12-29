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
using UnityEngine;
using Character = Amegakure.Starkane.EntitiesWrapper.Character;

public class Combat : MonoBehaviour
{
    private MatchState matchState;
    private Dictionary<Player, List<Character>> playerMatchCharacters = new();
    private Dictionary<Player, List<ActionState>> actionStates = new();
    private Dictionary<Player, List<CharacterState>> characterStates = new();

    public MatchState MatchState 
    { 
        get => matchState;
        set
        {
            matchState = value;
            matchState.playerTurnIdChanged += MatchState_playerTurnIdChanged;
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
    }

    private void Start()
    {
        int playerId = matchState.PlayerTurnId;
        Player playerTurn = playerMatchCharacters.Keys.First(p => p.Id == playerId);

        EventManager.Instance.Publish(GameEvent.COMBAT_TURN_CHANGED, new() { { "Player", playerTurn } });
    }


    private void OnDisable()
    {
        matchState.playerTurnIdChanged -= MatchState_playerTurnIdChanged;
    }

    private void MatchState_playerTurnIdChanged(int playerId)
    {
        Player playerTurn = playerMatchCharacters.Keys.First(p => p.Id == playerId);

        EventManager.Instance.Publish(GameEvent.COMBAT_TURN_CHANGED, new() { { "Player", playerTurn } });
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
    }

    public void Move(Character character, Player player, Tile target)
    {
        if (CanMove(character, player))
        {
            CallMoveTx(character, player, target);
            character.Move(target);
        }
    }

    private void CallMoveTx(Character character, Player player, Tile target)
    {
        string rpcUrl = "http://localhost:5050";

        var provider = new JsonRpcClient(rpcUrl);
        var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
        string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

        var account = new Account(provider, signer, playerAddress);
        string actionsAddress = "0xf95f269a39505092b2d4eea3268e2e8da83cfd12a20b0eceb505044ecaabf2";
        
        string match_id_string = matchState.Id.ToString("X");
        var match_id = dojo.felt_from_hex_be(new CString(match_id_string)).ok;
        
        string player_id_string = player.Id.ToString("X");
        var player_id = dojo.felt_from_hex_be(new CString(player_id_string)).ok;
        
        string character_id_string = character.GetId().ToString("X");
        var character_id = dojo.felt_from_hex_be(new CString(character_id_string)).ok;
        
        string x_string = target.coordinate.x.ToString("X");
        var x = dojo.felt_from_hex_be(new CString(x_string)).ok;

        string y_string = target.coordinate.y.ToString("X");
        var y = dojo.felt_from_hex_be(new CString(y_string)).ok;

        dojo.Call call = new dojo.Call()
        {
            calldata = new[]
            {
                   match_id, player_id, character_id, x, y
                },
            to = actionsAddress,
            selector = "move"
        };

        account.ExecuteRaw(new[] { call });
    }

    public void CallEndTurnTX(Player player)
    {
         string rpcUrl = "http://localhost:5050";

            var provider = new JsonRpcClient(rpcUrl);
            var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
            string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

            var account = new Account(provider, signer, playerAddress);
            string actionsAddress = "0x61231db30a04f42b3c3e57cd13b0dee6053f8ed7c350135735e67c254b60454";
            
            List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();

            string match_id_string = matchState.Id.ToString("X");
            var match_id = dojo.felt_from_hex_be(new CString(match_id_string)).ok;
            
            string player_id_string = player.Id.ToString("X");
            var player_id = dojo.felt_from_hex_be(new CString(player_id_string)).ok;

            dojo.Call call = new dojo.Call()
            {
                calldata = new[]
                {
                   match_id, player_id
                },
                to = actionsAddress,
                selector = "end_turn"
            };
    
            account.ExecuteRaw(new[] { call });
    }

    public bool CanMove(Character character, Player player)
    {
        bool playerRegistered = playerMatchCharacters.ContainsKey(player) && playerMatchCharacters[player].Contains(character);

        if (playerRegistered)
        {
            bool characterMoved = GetActionState(player, character).Movement;
            return matchState.PlayerTurnId == player.Id && !characterMoved;
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
        int playerID = matchState.PlayerTurnId;
        foreach(Player player in playerMatchCharacters.Keys)
        {
            if(player.Id == playerID)
            {
                return player;
            }
        }
        
        return null;
    }

    public Player GetPlayerByID(int id) 
    {
        int playerID = matchState.PlayerTurnId;
        foreach (Player player in playerMatchCharacters.Keys)
        {
            if (player.Id == id)
            {
                return player;
            }
        }

        return null;
    }

    public void DoSkill()
    {

    }

    private void DoSkillTX()
    {

    }

    public bool CanDoSkill(Player player, Character character, Skill skillSelected)
    {
        bool playerRegistered = playerMatchCharacters.ContainsKey(player) && playerMatchCharacters[player].Contains(character);

        if (playerRegistered)
        {
            ActionState playerActionState = GetActionState(player, character);
            CharacterState playerCharacterState = GetCharacterState(player, character);

            bool skillPerformed = playerActionState.Action;
            bool enoughMana = playerCharacterState.Remain_mp >= skillSelected.Mp_cost;

            return !skillPerformed && enoughMana;
        }

        return false;
    }

    private CharacterState GetCharacterState(Player player, Character character)
    {
        return characterStates[player].Find(characterState => (int)characterState.Character_id == character.GetId());
    }
}
