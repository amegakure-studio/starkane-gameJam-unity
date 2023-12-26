using System.Collections.Generic;
using Amegakure.Starkane.Entities;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int id;
    private Dictionary<CharacterType, GameObject> m_CharacterDictionary = new();
    [SerializeField]private bool canBeDisplayed = true;
    [SerializeField] CharacterType defaultCharacter;

    public int Id { get => id; set => id = value; }
    public Dictionary<CharacterType, GameObject> CharacterDictionary { get => m_CharacterDictionary; private set => m_CharacterDictionary = value; }
    public bool CanBeDisplayed { get => canBeDisplayed; set => canBeDisplayed = value; }

    public void AddCharacter(CharacterType type, GameObject characterPrefab)
    {
        m_CharacterDictionary.Add(type, characterPrefab);
        if(type == defaultCharacter)
            characterPrefab.SetActive(true);
        else
            characterPrefab.SetActive(false);
    
    }

}
