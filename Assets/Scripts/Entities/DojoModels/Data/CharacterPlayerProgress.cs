using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Dojo;
using Dojo.Starknet;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class CharacterPlayerProgress : ModelInstance
{
    [ModelField("owner")]
    public FieldElement owner;
    
    [ModelField("character_id")]
    public UInt32 character_id;

    [ModelField("skin_id")]
    public UInt32 skin_id;

    [ModelField("owned")]
    public bool owned;

    [ModelField("level")]
    public UInt32 level;

    private BigInteger playerID;

    public CharacterType GetCharacterType()
    {
        return (CharacterType) character_id;
    }

    private Dictionary<CharacterType, string> characterPrefabsDict;

    public FieldElement Owner { get => owner; private set => owner = value; }

    private void Awake()
    {
        characterPrefabsDict =  new();
        characterPrefabsDict[CharacterType.Archer] = "Darkelf";
        characterPrefabsDict[CharacterType.Cleric] = "Wizard";
        characterPrefabsDict[CharacterType.Warrior] = "Avelyn";
        characterPrefabsDict[CharacterType.Goblin] = "Enemy";
    }

    //public override void Initialize(Model model)
    //{
    //    base.Initialize(model);

    //    Debug.Log(owner.Hex());
    //    Debug.Log(character_id);
    //    Debug.Log(skin_id);
    //    Debug.Log(owned);
    //    Debug.Log(level);
    //}

    public BigInteger getPlayerID()
    {
        Debug.Log(owner.Hex());
        playerID = BigInteger.Parse(owner.Hex(), NumberStyles.AllowHexSpecifier);

        return playerID;
    }

}
