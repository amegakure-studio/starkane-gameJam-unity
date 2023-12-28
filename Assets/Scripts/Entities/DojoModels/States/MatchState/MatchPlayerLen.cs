using System;
using Dojo;
using Dojo.Torii;
using UnityEngine;

public class MatchPlayerLen : ModelInstance
{
    private UInt32 match_id;
    private UInt32 players_len;

    public override void Initialize(Model model)
    {
        match_id = model.members["match_id"].ty.ty_primitive.u32;
        players_len = model.members["players_len"].ty.ty_primitive.u32;

        Debug.Log("MatchPlayerLen: \n match_id: " + match_id + "\n players_len: " + players_len + "\n");
    }
}
