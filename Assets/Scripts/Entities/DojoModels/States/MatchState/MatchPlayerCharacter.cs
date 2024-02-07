using System;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using Dojo.Starknet;
using UnityEngine;

public class MatchPlayerCharacter : ModelInstance
{
    [ModelField("match_id")]
    public UInt32 match_id;
    
    [ModelField("player")]
    public FieldElement player;
    
    [ModelField("id")]
    public UInt32 id;
    
    [ModelField("character_id")]
    public UInt32 character_id;

    private BigInteger playerId;

    public uint Match_id { get => match_id; set => match_id = value; }
    
    public BigInteger PlayerId { get
                                    {
                                        //var playerString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();
                                        //playerId = BigInteger.Parse(playerString, System.Globalization.NumberStyles.AllowHexSpecifier);
                                        return playerId;
                                    }
                                    set => playerId = value;
                                }
    
    public uint Character_id { get => character_id; set => character_id = value; }

}
