using System.Collections.Generic;
using Amegakure.Starkane.Entities;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int id;
    private Dictionary<CharacterType, GameObject> m_CharacterDictionary = new();

    public int Id { get => id; set => id = value; }

    public void AddCharacter(CharacterType type, GameObject characterPrefab)
    {
        m_CharacterDictionary.Add(type, characterPrefab);
        Debug.Log("KEYS COUNT:" + m_CharacterDictionary.Keys.Count);
    }

}
