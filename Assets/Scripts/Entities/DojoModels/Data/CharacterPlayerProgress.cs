using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Amegakure.Starkane.Entities;
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
    private BigInteger playerID;

    public CharacterType GetCharacterType()
    {
        return (CharacterType) character_id;
    }

    private Dictionary<CharacterType, string> characterPrefabsDict;

    public dojo.FieldElement Owner { get => owner; private set => owner = value; }

    private void Awake()
    {
        characterPrefabsDict =  new();
        characterPrefabsDict[CharacterType.Archer] = "Darkelf";
        characterPrefabsDict[CharacterType.Cleric] = "Wizard";
        characterPrefabsDict[CharacterType.Warrior] = "Avelyn";
        characterPrefabsDict[CharacterType.Goblin] = "Enemy";
    }

    public override void Initialize(Model model)
    {
        owner = model.members["owner"].ty.ty_primitive.felt252;
        character_id = model.members["character_id"].ty.ty_primitive.u32;
        skin_id = model.members["skin_id"].ty.ty_primitive.u32;
        owned = model.members["owned"].ty.ty_primitive.p_bool;
        level = model.members["level"].ty.ty_primitive.u32;
        
        var hexString = BitConverter.ToString(owner.data.ToArray()).Replace("-", "").ToLower();
        playerID = BigInteger.Parse( hexString, NumberStyles.AllowHexSpecifier );
        // print();
    }

    public BigInteger getPlayerID()
    {
        return playerID;
    }

    public void print()
    {
        Debug.Log(" CPP: owner: " + playerID + "\n"
                  + "character_id: " + character_id + "\n" + "skin_id: " + skin_id + "\n" + "owned: " + owned
                  + "\n" + "level: " + level);
        
    }
}
