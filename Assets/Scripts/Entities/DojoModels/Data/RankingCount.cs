using System;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class RankingCount : ModelInstance
{
    [ModelField("id")]
    private dojo.FieldElement id;

    [ModelField("index")]
    private UInt32 index;
}
