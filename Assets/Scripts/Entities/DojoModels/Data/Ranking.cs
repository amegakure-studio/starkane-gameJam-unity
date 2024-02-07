using System;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class Ranking : ModelInstance
{
    [ModelField("id")]
    private UInt32 id;
    
    [ModelField("player")]
    private dojo.FieldElement player;
    
    private BigInteger playerID;
    
    [ModelField("score")]
    private UInt64 score;
}
