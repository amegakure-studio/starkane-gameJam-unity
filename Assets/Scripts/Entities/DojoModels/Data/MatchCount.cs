using System;
using Dojo;
using Dojo.Starknet;


public class MatchCount : ModelInstance
{
    [ModelField("index")]
    private UInt32 index;

    [ModelField("id")]
    private FieldElement id;
}
