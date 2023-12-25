using Amegakure.Starkane;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.Id;
using System;
using UnityEngine;

public class CharacterBuilder : MonoBehaviour
{
    [SerializeField] string charactersFolder = "Characters/";

    private GameObject characterGo;

    public CharacterBuilder AddCharacterPrefab(CharacterType characterType, string skinId)
    {
        string folderCharacterType = characterType.ToString() + "/";
        string path = charactersFolder + folderCharacterType + skinId;
            
            try
            {
                GameObject characterPrefab = Resources.Load<GameObject>(path);
                characterGo = Instantiate(characterPrefab);
                return this;
            }
            catch (Exception e) { Debug.LogError("Couldn't find characters folder: " + path + ". Details: " + e.Message); }      

        throw new ArgumentException("Couldn't create character");
    }

    public CharacterBuilder AddCharacterController(CharacterId characterId)
    {
        if (characterGo)
        {
            CharacterController controller = characterGo.AddComponent<CharacterController>();
            controller.Id = characterId;
        }

        return this;
    }

    public GameObject Build()
    {
        return characterGo;
    }

}
