using System;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class MatchPlayerCharacterLen : ModelInstance
{
    private UInt32 match_id;
    private dojo.FieldElement player;
    private BigInteger playerId;
    private UInt32 characters_len;
    private UInt32 remain_characters;

    public override void Initialize(Model model)
    {
        match_id = model.members["match_id"].ty.ty_primitive.u32;
        player = model.members["player"].ty.ty_primitive.felt252;
        characters_len = model.members["characters_len"].ty.ty_primitive.u32;
        remain_characters = model.members["remain_characters"].ty.ty_primitive.u32;

        var playerString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();
        playerId = BigInteger.Parse(playerString, System.Globalization.NumberStyles.AllowHexSpecifier);

        // Debug.Log("MatchPlayerCharacterLen: \n match_id: " + match_id
        //           + "\n playerId: " + playerId
        //           + "\n characters_len: " + characters_len
        //           + "\n remain_characters: " + remain_characters + "\n");
    }
}
