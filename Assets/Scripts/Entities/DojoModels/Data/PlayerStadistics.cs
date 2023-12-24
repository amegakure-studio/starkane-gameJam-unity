using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using System;

public class PlayerStadistics : ModelInstance
{
    dojo.FieldElement owner;
    UInt64 matchs_won;
    UInt64 matchs_lost;
    UInt32 characters_owned;
    UInt64 total_score;

    public override void Initialize(Model model)
    {
        owner = model.members["owner"].ty.ty_primitive.felt252;
        matchs_won = model.members["matchs_won"].ty.ty_primitive.u64;
        matchs_lost = model.members["matchs_lost"].ty.ty_primitive.u64;
        characters_owned = model.members["characters_owned"].ty.ty_primitive.u32;
        total_score = model.members["total_score"].ty.ty_primitive.u64;
        
        var hexString = BitConverter.ToString(owner.data.ToArray()).Replace("-", "").ToLower();

        Debug.Log("owner: " + hexString + "\n"
                  + "matchs_won: " + matchs_won + "\n" + "matchs_lost: " + matchs_lost + "\n"
                  + "characters_owned: " + characters_owned
                  + "\n" + "total_score: " + total_score);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
