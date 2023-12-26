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

        private void Awake()
        {
            virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            Character character = GameObject.FindAnyObjectByType<Character>();
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
