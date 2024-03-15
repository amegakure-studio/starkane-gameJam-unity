using System;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using Dojo.Starknet;
using UnityEngine;

public class MatchPlayer : ModelInstance
{
    [ModelField("match_id")]
    public UInt32 match_id;

    [ModelField("id")]
    public UInt32 id;

    [ModelField("player")]
    public FieldElement player;


    public uint Match_id { get => match_id; private set => match_id = value; }


}
