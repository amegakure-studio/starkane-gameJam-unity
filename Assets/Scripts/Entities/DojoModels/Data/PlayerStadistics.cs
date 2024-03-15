using Dojo;
using Dojo.Starknet;
using Dojo.Torii;
using dojo_bindings;
using System;
using System.Globalization;
using System.Numerics;

public class PlayerStadistics : ModelInstance
{
    [ModelField("owner")]
    public FieldElement owner;

    [ModelField("matchs_won")]
    public UInt64 matchs_won;

    [ModelField("matchs_lost")]
    public UInt64 matchs_lost;

    [ModelField("characters_owned")]
    public UInt32 characters_owned;

    [ModelField("total_score")]
    public UInt64 total_score;
}
