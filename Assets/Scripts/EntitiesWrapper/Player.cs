using System.Collections.Generic;
using Amegakure.Starkane.Entities;
using dojo_bindings;
using UnityEngine;

namespace Amegakure.Starkane.EntitiesWrapper
{
    public class Player : MonoBehaviour
    {
        [SerializeField] int id;
        [SerializeField] string playerName;
        [SerializeField] CharacterType defaultCharacter;
        
        private dojo.FieldElement owner;
        private List<Character> characters;

        public int Id { get => id; set => id = value; }
        public string PlayerName { get => playerName; set => playerName = value; }
        public CharacterType DefaultCharacter { get => defaultCharacter; private set => defaultCharacter = value; }
        public List<Character> Characters { get => characters; set => characters = value; }

        private void Awake()
        {
            characters = new();
        }

        public void SetDojoId(dojo.FieldElement id)
        {
            owner = id;
        }
    }

}
