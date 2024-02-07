using System;
using Dojo;
using Dojo.Torii;
using UnityEngine;

public class MatchPlayerLen : ModelInstance
{
    [ModelField("match_id")]
    private UInt32 match_id;

    [ModelField("players_len")]
    private UInt32 players_len;
}
