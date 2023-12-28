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
        private Character character;

        private void Start()
        {
            LoadCharacter();
            virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();     
        }

        private void Update()
        {
            if (character == null)
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
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_SELECTED, handleCharacterSelected);
        }

        private void handleCharacterSelected(Dictionary<string, object> context)
        {
            Character character = (Character) context["Character"];
            FocusCharacter(character);
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
