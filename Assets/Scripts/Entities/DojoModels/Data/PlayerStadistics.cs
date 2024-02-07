using Dojo;
using Dojo.Torii;
using dojo_bindings;
using System;
using System.Globalization;
using System.Numerics;

public class PlayerStadistics : ModelInstance
{
    [ModelField("owner")]
    dojo.FieldElement owner;

    [ModelField("matchs_won")]
    UInt64 matchs_won;

    [ModelField("matchs_lost")]
    UInt64 matchs_lost;

    [ModelField("characters_owned")]
    UInt32 characters_owned;

    [ModelField("total_score")]
    UInt64 total_score;
}
