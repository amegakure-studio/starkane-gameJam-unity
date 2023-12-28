using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character = Amegakure.Starkane.EntitiesWrapper.Character;

public class Combat : MonoBehaviour
{
    private MatchState matchState;
    private Dictionary<Player, List<Character>> playerMatchCharacters = new();
    private Dictionary<Player, List<ActionState>> actionStates = new();
    private Dictionary<Player, List<CharacterState>> characterStates = new();

    public MatchState MatchState { get => matchState; set => matchState = value; }
    public Dictionary<Player, List<Character>> PlayerMatchCharacter { get => playerMatchCharacters; private set => playerMatchCharacters = value; }
    public Dictionary<Player, List<ActionState>> ActionStates { get => actionStates; private set => actionStates = value; }
    public Dictionary<Player, List<CharacterState>> CharacterStates { get => characterStates; private set => characterStates = value; }

    private void Awake()
    {
        playerMatchCharacters = new();
        actionStates = new();
        characterStates = new();
    }

    public void AddCharacter(Player player, Character character, ActionState actionState, CharacterState characterState)
    {
        if (!playerMatchCharacters.ContainsKey(player))
            playerMatchCharacters[player] = new();

        List <Character> currentCharacters = playerMatchCharacters[player];
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
            character.Move(target);
        }
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
}
