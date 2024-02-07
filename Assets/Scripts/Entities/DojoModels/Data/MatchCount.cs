using System;
using Dojo;
using Dojo.Starknet;


public class MatchCount : ModelInstance
{
    [ModelField("index")]
    public UInt32 index;

    [ModelField("id")]
    public FieldElement id;
}
