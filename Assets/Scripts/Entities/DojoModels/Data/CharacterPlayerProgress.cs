using System;
using System.Collections.Generic;
using System.Globalization;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.Id;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class CharacterPlayerProgress : ModelInstance
{
    dojo.FieldElement owner;
    UInt32 character_id;
    UInt32  skin_id;
    bool owned;
    UInt32 level;
    private int intID;

    private Dictionary<CharacterType, string> characterPrefabsDict;

    private void Awake()
    {
        characterPrefabsDict =  new();
        characterPrefabsDict[CharacterType.Warrior] = "Avelyn";
        characterPrefabsDict[CharacterType.Pig] = "Enemy";
        characterPrefabsDict[CharacterType.Archer] = "Darkelf";
    }

    public override void Initialize(Model model)
    {
        owner = model.members["owner"].ty.ty_primitive.felt252;
        character_id = model.members["character_id"].ty.ty_primitive.u32;
        skin_id = model.members["skin_id"].ty.ty_primitive.u32;
        owned = model.members["owned"].ty.ty_primitive.p_bool;
        level = model.members["level"].ty.ty_primitive.u32;
        
        var hexString = BitConverter.ToString(owner.data.ToArray()).Replace("-", "").ToLower();
        intID = System.Int32.Parse( hexString, NumberStyles.AllowHexSpecifier );
        
        CharacterType characterType = (CharacterType) character_id;
        GameObject builderGo = Instantiate(new GameObject());
        CharacterBuilder builder = builderGo.AddComponent<CharacterBuilder>();
        GameObject[] playerGoList = GameObject.FindGameObjectsWithTag("Player");
        
        foreach( GameObject playerGo in playerGoList)
        {
            try
            {
                playerGo.TryGetComponent<Player>(out Player player);
                if(player != null)
                {
                    if(player.Id == intID)
                    {
                        CharacterId id = new CharacterId(player.Id, characterType);
                        GameObject characterGo = builder
                                .AddCharacterPrefab(characterType, characterPrefabsDict[characterType])
                                .AddCharacterController(id)
                                .AddGridMovement(id)
                                .Build();

                        id.CharacterGo = characterGo;
                        
                        player.AddCharacter(characterType, characterGo);
                        characterGo.transform.parent = playerGo.transform;
                        gameObject.transform.parent = playerGo.transform;

                        characterGo.SetActive(player.CanBeDisplayed);
                        player.CanBeDisplayed = false;
                    }   
                }        
            }
            catch(Exception e){Debug.LogError(e);}
        }

        Destroy( builderGo );
    }

    public void print()
    {
        Debug.Log("owner: " + intID + "\n"
                  + "character_id: " + character_id + "\n" + "skin_id: " + skin_id + "\n" + "owned: " + owned
                  + "\n" + "level: " + level);
        
    }
}
