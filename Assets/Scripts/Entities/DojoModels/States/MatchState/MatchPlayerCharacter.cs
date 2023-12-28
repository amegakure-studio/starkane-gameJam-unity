using System;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class MatchPlayerCharacter : ModelInstance
{
    private UInt32 match_id;
    private dojo.FieldElement player;
    private int playerId;
    private UInt32 id;
    private UInt32 character_id;

    public override void Initialize(Model model)
    {
        match_id = model.members["match_id"].ty.ty_primitive.u32;
        player = model.members["player"].ty.ty_primitive.felt252;
        id = model.members["id"].ty.ty_primitive.u32;
        character_id = model.members["character_id"].ty.ty_primitive.u32;

        var playerString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();
        playerId = System.Int32.Parse(playerString, System.Globalization.NumberStyles.AllowHexSpecifier);

        Debug.Log("MatchPlayerCharacter: \n match_id: " + match_id
                  + "\n playerId: " + playerId
                  + "\n id: " + id
                  + "\n character_id: " + character_id + "\n");
    }
}
