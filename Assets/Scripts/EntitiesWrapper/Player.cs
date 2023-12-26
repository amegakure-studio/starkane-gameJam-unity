using System.Collections.Generic;
using Amegakure.Starkane.Entities;
using UnityEngine;

namespace Amegakure.Starkane.EntitiesWrapper
{
    public class Player : MonoBehaviour
    {
        [SerializeField] int id;
        [SerializeField] string playerName;
        [SerializeField] CharacterType defaultCharacter;

        public int Id { get => id; set => id = value; }
        public string PlayerName { get => playerName; set => playerName = value; }
        public CharacterType DefaultCharacter { get => defaultCharacter; private set => defaultCharacter = value; }

        // [SerializeField] int id;
        // private Dictionary<CharacterType, GameObject> m_CharacterDictionary = new();
        // [SerializeField]private bool canBeDisplayed = true;
        // [SerializeField] CharacterType defaultCharacter;

        // public int Id { get => id; set => id = value; }
        // public Dictionary<CharacterType, GameObject> CharacterDictionary { get => m_CharacterDictionary; private set => m_CharacterDictionary = value; }
        // public bool CanBeDisplayed { get => canBeDisplayed; set => canBeDisplayed = value; }
        // public CharacterType DefaultCharacter { get => defaultCharacter; private set => defaultCharacter = value; }

        // public void AddCharacter(CharacterType type, GameObject characterPrefab)
        // {
        //     m_CharacterDictionary.Add(type, characterPrefab);
        //     if(type == defaultCharacter)
        //         characterPrefab.SetActive(true);
        //     else
        //         characterPrefab.SetActive(false);
        // }

    }

}
