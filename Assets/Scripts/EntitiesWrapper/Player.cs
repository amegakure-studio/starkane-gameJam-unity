using System.Collections.Generic;
using System.Numerics;
using Amegakure.Starkane.Entities;
using Dojo.Starknet;
using dojo_bindings;
using UnityEngine;

namespace Amegakure.Starkane.EntitiesWrapper
{
    public class Player : MonoBehaviour
    {
        [SerializeField] BigInteger id;
        [SerializeField] string playerName;
        [SerializeField] CharacterType defaultCharacter;
        
        private FieldElement owner;
        private List<Character> characters;

        public BigInteger Id { get => id; set => id = value; }
        public string PlayerName { get => playerName; set => playerName = value; }
        public CharacterType DefaultCharacter { get => defaultCharacter; set => defaultCharacter = value; }
        public List<Character> Characters { get => characters; set => characters = value; }

        private void Awake()
        {
            characters = new();
        }

        public void SetDojoId(FieldElement id)
        {
            owner = id;
        }
    }

}
