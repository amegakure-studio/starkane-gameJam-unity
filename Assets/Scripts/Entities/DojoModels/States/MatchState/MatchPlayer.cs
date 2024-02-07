using System;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using Dojo.Starknet;
using UnityEngine;

public class MatchPlayer : ModelInstance
{
    [ModelField("match_id")]
    public UInt32 match_id;

    [ModelField("id")]
    public UInt32 id;

    [ModelField("player")]
    public FieldElement player;

    public BigInteger playerId;

    public uint Match_id { get => match_id; private set => match_id = value; }
    public BigInteger PlayerId { get
                                    {
                                        //var playerString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();
                                        //playerId = BigInteger.Parse(playerString, System.Globalization.NumberStyles.AllowHexSpecifier);
            
                                        return playerId;
                                    }
                                    private set => playerId = value;
                               }

}
