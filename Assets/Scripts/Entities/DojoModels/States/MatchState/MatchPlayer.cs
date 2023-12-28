using System;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class MatchPlayer : ModelInstance
{
    private UInt32 match_id;
    private UInt32 id;
    private dojo.FieldElement player;
    private int playerId;

    public uint Match_id { get => match_id; private set => match_id = value; }
    public int PlayerId { get => playerId; private set => playerId = value; }

    public override void Initialize(Model model)
    {
        match_id = model.members["match_id"].ty.ty_primitive.u32;
        id = model.members["id"].ty.ty_primitive.u32;
        player = model.members["player"].ty.ty_primitive.felt252;

        var playerString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();
        playerId = System.Int32.Parse(playerString, System.Globalization.NumberStyles.AllowHexSpecifier);

        // Debug.Log("MatchPlayer: \n match_id: " + match_id + "\n id: " + id + "\n playerId: " + playerId + "\n");
    }
}
