using Amegakure.Starkane.Entities;
using Amegakure.Starkane.GridSystem;
using System;
using UnityEngine;

public class CharacterBuilder : MonoBehaviour
{
    [SerializeField] string charactersFolder = "Characters/";

    private GameObject characterGo;
    private Context context;

    private void Awake()
    {   
        context = GameObject.FindObjectOfType<Context>();
    }

    public CharacterBuilder AddCharacterPrefab(CharacterType characterType, string skinId, CharacterPlayerProgress characterPlayerProgress)
    {
        string folderCharacterType = characterType.ToString() + "/";
        string path = charactersFolder + folderCharacterType + skinId;
            
            try
            {
                GameObject characterPrefab = Resources.Load<GameObject>(path);
                characterGo = Instantiate(characterPrefab);
                
                Amegakure.Starkane.EntitiesWrapper.Character character
                                = characterGo.AddComponent<Amegakure.Starkane.EntitiesWrapper.Character>();
                
                character.Location = context.GetInitialLocation();
                character.CharacterPlayerProgress = characterPlayerProgress;

                return this;
            }
            catch (Exception e) { Debug.LogError("Couldn't find characters folder: " + path + ". Details: " + e.Message); }      

        throw new ArgumentException("Couldn't create character");
    }

    public CharacterBuilder AddCombatElements( CharacterState characterState, ActionState actionState)
    {
        try
        {
            Debug.Log(characterGo.name);
            Amegakure.Starkane.EntitiesWrapper.Character character
                                = characterGo.GetComponent<Amegakure.Starkane.EntitiesWrapper.Character>();
        
            character.ActionState = actionState;
            character.CharacterState = characterState;
            return this;
        }
        catch(Exception e){ Debug.LogError("Couldn't add elements to character" + e);}
        
        throw new ArgumentException("Couldn't add elements to character");
    }

    public CharacterBuilder AddCharacterController()
    {
        if (characterGo)
        {
            CharacterController controller = characterGo.AddComponent<CharacterController>();
            Amegakure.Starkane.EntitiesWrapper.Character character
                                = characterGo.GetComponent<Amegakure.Starkane.EntitiesWrapper.Character>();
            
            controller.Character = character;
        }

        return this;
    }

    public CharacterBuilder AddGridMovement()
    {   
        if(characterGo)
        {
            GridMovement movement = characterGo.AddComponent<GridMovement>();
            movement.TileStyle = PathStyle.SQUARE;
            movement.NumOfTiles = 50;
            movement.Speed = 2;

            Amegakure.Starkane.EntitiesWrapper.Character character
                                = characterGo.GetComponent<Amegakure.Starkane.EntitiesWrapper.Character>();
            character.GridMovement = movement;
        }

        return this;
    }

    public GameObject Build()
    {
        return characterGo;
    }

}
