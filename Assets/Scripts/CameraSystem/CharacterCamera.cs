using System;
using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using Cinemachine;
using UnityEngine;

namespace Amegakure.Starkane.CameraSystem
{
    public class CharacterCamera : MonoBehaviour
    {
        private CinemachineVirtualCamera virtualCamera;
        private Character currentCharacter;
        Combat combat;

        private void Start()
        {
            virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();     
            LoadCharacter();
        }

        private void Update()
        {
            if (currentCharacter == null)
                LoadCharacter();
        }

        private void LoadCharacter()
        { 
            Character character = GameObject.FindFirstObjectByType<Character>();
            FocusCharacter(character);
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_SELECTED, handleCharacterSelected);
            EventManager.Instance.Subscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_SELECTED, handleCharacterSelected);
            EventManager.Instance.Unsubscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
        }

        private void HandleCombatTurnChanged(Dictionary<string, object> context)
        {
            if (combat != null)
                combat = FindAnyObjectByType<Combat>();

            try
            {
                Player playerTurn = (Player)context["Player"];
                
                if(combat == null)
                        combat = GameObject.FindAnyObjectByType<Combat>();

                List<Character> charactersTurn = combat.GetCharacters(playerTurn);
                
                Character character =charactersTurn[0];
                // Debug.Log("Turn: player" + playerTurn.Id);
                FocusCharacter(character);
                
            }
            catch { }

        }

        private void handleCharacterSelected(Dictionary<string, object> context)
        {
            Character character = (Character) context["Character"];
            FocusCharacter(character);
            currentCharacter = character;
        }

        private void FocusCharacter(Character character)
        {
            if (character)
            {
                virtualCamera.LookAt = character.transform;
                virtualCamera.Follow = character.transform;
            }  
        }
    }

}
