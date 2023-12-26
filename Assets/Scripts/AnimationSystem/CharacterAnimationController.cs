using System.Collections.Generic;
using UnityEngine;
using Amegakure.Starkane.PubSub;
using Amegakure.Starkane.EntitiesWrapper;

namespace Amegakure.Starkane.AnimationSystem
{
    public class CharacterAnimationController : MonoBehaviour
    {
        private Dictionary<Character, Animator> characterAnimatorMap;

        private void Awake()
        {
            characterAnimatorMap = new();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.CHARACTER_MOVE_START, HandleCharacterMoveStart);
            EventManager.Instance.Subscribe(GameEvent.CHARACTER_MOVE_END, HandleCharacterMoveEnd);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.CHARACTER_MOVE_START, HandleCharacterMoveStart);
            EventManager.Instance.Unsubscribe(GameEvent.CHARACTER_MOVE_END, HandleCharacterMoveEnd);
        }

        private void HandleCharacterMoveStart(Dictionary<string, object> context)
        {
            try
            {
                Character character = (Character) context["Character"];


                if (!characterAnimatorMap.ContainsKey(character))
                    characterAnimatorMap[character] = character.GetComponent<Animator>();

                Animator animator = characterAnimatorMap[character];
                animator?.SetBool("IsWalking", true);

            } catch {}
        }

        private void HandleCharacterMoveEnd(Dictionary<string, object> context)
        {
            try
            {
                Character character = (Character) context["Character"];

                if (characterAnimatorMap.ContainsKey(character))
                {
                    Animator animator = characterAnimatorMap[character];
                    animator?.SetBool("IsWalking", false);
                }

            } catch {}
        }
    }
}
