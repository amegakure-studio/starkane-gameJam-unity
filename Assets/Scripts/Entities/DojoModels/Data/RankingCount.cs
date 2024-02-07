using System;
using Dojo;
using Dojo.Starknet;

public class RankingCount : ModelInstance
{
    [ModelField("id")]
    public FieldElement id;

    [ModelField("index")]
    public UInt32 index;
}
