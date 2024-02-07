using System;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using Dojo.Starknet;
using UnityEngine;

public class ActionState : ModelInstance
{
    [ModelField("match_id")]
    public UInt32 match_id;

    [ModelField("character_id")]
    public UInt32 character_id;

    // Check this
    [ModelField("player")]
    public FieldElement player;

    [ModelField("action")]
    public bool action;

    [ModelField("movement")]
    public bool movement;

    public BigInteger playerId;

    public uint Match_id { get => match_id; set => match_id = value; }
    public uint Character_id { get => character_id; set => character_id = value; }
    public BigInteger Player_id { get 
                                        {
                                            //var playerString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();
                                            //playerId = BigInteger.Parse(playerString, System.Globalization.NumberStyles.AllowHexSpecifier);
                                            return playerId;
                                        }
                                    set => playerId = value;
                                }
    public bool Action { get => action; set => action = value; }
    public bool Movement { get => movement; set => movement = value; }
}
