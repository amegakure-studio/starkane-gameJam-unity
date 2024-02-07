using System;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class MatchPlayerCharacterLen : ModelInstance
{
    [ModelField("match_id")]
    private UInt32 match_id;

    [ModelField("player")]
    private dojo.FieldElement player;
    
    [ModelField("characters_len")] 
    private UInt32 characters_len;

    [ModelField("remain_characters")] 
    private UInt32 remain_characters;
}
