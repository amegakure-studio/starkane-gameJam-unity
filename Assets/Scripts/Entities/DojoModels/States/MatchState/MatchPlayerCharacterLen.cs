using System;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using Dojo.Starknet;
using UnityEngine;

public class MatchPlayerCharacterLen : ModelInstance
{
    [ModelField("match_id")]
    public UInt32 match_id;

    [ModelField("player")]
    public FieldElement player;
    
    [ModelField("characters_len")]
    public UInt32 characters_len;

    [ModelField("remain_characters")]
    public UInt32 remain_characters;
}
