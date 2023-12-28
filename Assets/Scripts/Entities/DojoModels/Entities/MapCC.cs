using System;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class MapCC : ModelInstance
{
    private UInt32 id;
    private UInt64 token_id;
    private byte size;
    private dojo.FieldElement obstacles_1;
    private dojo.FieldElement obstacles_2;
    private dojo.FieldElement obstacles_3;
    private dojo.FieldElement owner;
    private UInt64 width;
    private UInt64 height;

    public override void Initialize(Model model)
    {
        id = model.members["id"].ty.ty_primitive.u32;
        token_id = model.members["token_id"].ty.ty_primitive.u64;
        size = model.members["size"].ty.ty_primitive.u8;
        obstacles_1 = model.members["obstacles_1"].ty.ty_primitive.felt252;
        obstacles_2 = model.members["obstacles_2"].ty.ty_primitive.felt252;
        obstacles_3 = model.members["obstacles_3"].ty.ty_primitive.felt252;
        owner = model.members["owner"].ty.ty_primitive.felt252;
        width = model.members["width"].ty.ty_primitive.u64;
        height = model.members["height"].ty.ty_primitive.u64;

        // Debug.Log("MapCC: \n id: " + id
        //           + "\n token_id: " + token_id
        //           + "\n size: " + size
        //           + "\n obstacles_1: " + obstacles_1
        //           + "\n obstacles_2: " + obstacles_2
        //           + "\n obstacles_3: " + obstacles_3
        //           + "\n owner: " + owner
        //           + "\n width: " + width
        //           + "\n height: " + height + "\n");
    }
}
