using System.Collections.Generic;
using UnityEngine;
using Amegakure.Starkane.PubSub;
using Amegakure.Starkane.EntitiesWrapper;
using System;

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
            EventManager.Instance.Subscribe(GameEvent.COMBAT_CHARACTER_DEAD, HandleCharacterDead);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.CHARACTER_MOVE_START, HandleCharacterMoveStart);
            EventManager.Instance.Unsubscribe(GameEvent.CHARACTER_MOVE_END, HandleCharacterMoveEnd);
            EventManager.Instance.Unsubscribe(GameEvent.COMBAT_CHARACTER_DEAD, HandleCharacterDead);
        }

        private void HandleCharacterDead(Dictionary<string, object> context)
        {
            try
            {
                Character character = (Character)context["Character"];


                if (!characterAnimatorMap.ContainsKey(character))
                    characterAnimatorMap[character] = character.GetComponent<Animator>();

                Animator animator = characterAnimatorMap[character];
                animator?.SetBool("IsDead", true);

            }
            catch { }
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
