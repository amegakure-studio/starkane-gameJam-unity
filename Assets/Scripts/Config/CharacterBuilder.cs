using Amegakure.Starkane.Context;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.Id;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBuilder : MonoBehaviour
{
    [SerializeField] string charactersFolder = "Characters/";

    private GameObject characterGo;
    private Map map;

    private void Awake()
    {   
        map = GameObject.FindObjectOfType<Map>();
    }

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

    public CharacterBuilder AddGridMovement(CharacterId characterId)
    {   
        if(characterGo)
        {
            WorldCharacterContext context = characterGo.AddComponent<WorldCharacterContext>();
            context.Id = characterId;
            
            Tile location = map.DefaultWorldMapTile;
            location.OccupyingObject = characterGo;
            characterGo.transform.position = location.transform.position;
            context.Location = location;

            GridMovement movement = characterGo.AddComponent<GridMovement>();
            movement.TileStyle = PathStyle.SQUARE;
            movement.NumOfTiles = 50;
            movement.Speed = 8;
        }

        return this;
    }

    public GameObject Build()
    {
        return characterGo;
    }

}
