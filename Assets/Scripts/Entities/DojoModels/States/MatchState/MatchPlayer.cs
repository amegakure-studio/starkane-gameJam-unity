using System;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class MatchPlayer : ModelInstance
{
    [ModelField("match_id")]
    private UInt32 match_id;

    [ModelField("id")]
    private UInt32 id;

    [ModelField("player")]
    private dojo.FieldElement player;

    private BigInteger playerId;

    public uint Match_id { get => match_id; private set => match_id = value; }
    public BigInteger PlayerId { get
                                    {
                                        var playerString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();
                                        playerId = BigInteger.Parse(playerString, System.Globalization.NumberStyles.AllowHexSpecifier);
            
                                        return playerId;
                                    }
                                    private set => playerId = value;
                               }

}
