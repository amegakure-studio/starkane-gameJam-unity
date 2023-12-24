using Amegakure.Starkane.Entities;
using System;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    [SerializeField] static string charactersFolder = "Characters/";

    public static GameObject Create(CharacterType characterType, string skinId)
    {
        string folderCharacterType = "";

        switch(characterType)
        {
            case CharacterType.Archer:
                folderCharacterType = "Archer/";
                break;
            case CharacterType.Cleric:
                folderCharacterType = "Cleric/";
                break;
            case CharacterType.Warrior:
                folderCharacterType = "Warrior/";
                break;
            case CharacterType.Pig:
                folderCharacterType = "Pig/";
                break;
            case CharacterType.Peasant:
                folderCharacterType = "Peasant/";
                break;
            default: break;
        }

        if (!string.IsNullOrWhiteSpace(folderCharacterType))
        {
            string path = charactersFolder + folderCharacterType + skinId;
            
            try
            {
                GameObject characterPrefab = Resources.Load<GameObject>(path);
                return Instantiate(characterPrefab);
            }
            catch (Exception e) { Debug.LogError("Couldn't find characters folder: " + path + ". Details: " + e.Message); }      
        }

        throw new ArgumentException("Couldn't create character");
    }
}
