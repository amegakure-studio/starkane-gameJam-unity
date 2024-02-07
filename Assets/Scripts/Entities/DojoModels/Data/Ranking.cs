using System;
using System.Numerics;
using Dojo;
using Dojo.Starknet;

public class Ranking : ModelInstance
{
    [ModelField("id")]
    public UInt32 id;
    
    [ModelField("player")]
    public FieldElement player;

    public BigInteger playerID;
    
    [ModelField("score")]
    public UInt64 score;
}
