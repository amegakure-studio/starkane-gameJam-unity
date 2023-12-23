using System.Collections.Generic;
using UnityEngine;
using Amegakure.Starkane.Entities;

namespace Amegakure.Starkane.AnimationSystem
{
    public class CharacterAnimationController : MonoBehaviour
    {
        private Dictionary<Character, Animator> characterAnimatorMap;

        private void Awake()
        {
            characterAnimatorMap = new Dictionary<Character, Animator>();
        }

        private void Start()
        {
            Character[] characters = GameObject.FindObjectsByType<Character>(FindObjectsSortMode.InstanceID);

            foreach (Character character in characters)
            {
                Animator animator = character.GetComponent<Animator>();
                characterAnimatorMap[character] = animator;
            }
        }
    }
}
