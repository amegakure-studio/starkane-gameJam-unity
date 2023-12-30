using System;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class ActionState : ModelInstance
{
    private UInt32 match_id;
    private UInt32 character_id;
    private dojo.FieldElement player;
    private int playerId;
    private bool action;
    private bool movement;

    public uint Match_id { get => match_id; set => match_id = value; }
    public uint Character_id { get => character_id; set => character_id = value; }
    public int Player_id { get => playerId; set => playerId = value; }
    public bool Action { get => action; set => action = value; }
    public bool Movement { get => movement; set => movement = value; }

    public override void Initialize(Model model)
    {
        match_id = model.members["match_id"].ty.ty_primitive.u32;
        character_id = model.members["character_id"].ty.ty_primitive.u32;
        player = model.members["player"].ty.ty_primitive.felt252;
        action = model.members["action"].ty.ty_primitive.p_bool;
        movement = model.members["movement"].ty.ty_primitive.p_bool;

        var playerString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();
        playerId = System.Int32.Parse(playerString, System.Globalization.NumberStyles.AllowHexSpecifier);

        Debug.Log("ActionState: \n match_id: " + match_id
                  + "\n character_id: " + character_id
                  + "\n playerId: " + playerId
                  + "\n action: " + action
                  + "\n movement: " + movement + "\n");
    }
}
