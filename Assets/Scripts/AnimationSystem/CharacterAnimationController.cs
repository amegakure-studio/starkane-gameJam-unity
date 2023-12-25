using System.Collections.Generic;
using UnityEngine;
using Amegakure.Starkane.Entities;
using System.Linq;
using Amegakure.Starkane.Id;

namespace Amegakure.Starkane.AnimationSystem
{
    public class CharacterAnimationController : MonoBehaviour
    {
        private Dictionary<CharacterId, Animator> characterAnimatorMap;

        private void Awake()
        {
            characterAnimatorMap = new();
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
