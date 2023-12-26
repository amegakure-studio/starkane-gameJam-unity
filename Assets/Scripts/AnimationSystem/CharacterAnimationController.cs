using System.Collections.Generic;
using UnityEngine;
using Amegakure.Starkane.Entities;
using System.Linq;
using Amegakure.Starkane.Id;
using Amegakure.Starkane.PubSub;
using System;
using Amegakure.Starkane.Context;

namespace Amegakure.Starkane.AnimationSystem
{
    public class CharacterAnimationController : MonoBehaviour
    {
        private Dictionary<CharacterId, Animator> characterAnimatorMap;

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
                WorldCharacterContext characterContext = (WorldCharacterContext) context["Character"];

                if (characterAnimatorMap.ContainsKey(characterContext.Id))
                {
                    Animator animator = characterAnimatorMap[characterContext.Id];
                    animator.SetBool("IsWalking", true);
                }

            } catch {}
        }

        private void HandleCharacterMoveEnd(Dictionary<string, object> context)
        {
            try
            {
                WorldCharacterContext characterContext = (WorldCharacterContext) context["Character"];

                if (characterAnimatorMap.ContainsKey(characterContext.Id))
                {
                    Animator animator = characterAnimatorMap[characterContext.Id];
                    animator.SetBool("IsWalking", false);
                }

            } catch {}
        }

        private void Start()
        {
            RegisterPlayers();        
        }

        private void RegisterPlayers()
        {
            List<Player> players = GameObject.FindObjectsOfType<Player>().ToList();
            players.ForEach(player => RegisterCharacter(player));
        }

        private void RegisterCharacter(Player player)
        {
            foreach (CharacterType characterType in player.CharacterDictionary.Keys)
            {
                GameObject character = player.CharacterDictionary[characterType];
                Animator animator = character.GetComponent<Animator>();
                CharacterId id = new(player.Id, characterType);

                characterAnimatorMap[id] = animator;
            }
        }
    }
}
