using System;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class CharacterPlayerProgress : ModelInstance
{
    dojo.FieldElement owner;
    UInt32 character_id;
    UInt32  skin_id;
    bool owned;
    UInt32 level;


    public override void Initialize(Model model)
    {
        owner = model.members["owner"].ty.ty_primitive.felt252;
        character_id = model.members["character_id"].ty.ty_primitive.u32;
        skin_id = model.members["skin_id"].ty.ty_primitive.u32;
        owned = model.members["owned"].ty.ty_primitive.p_bool;
        level = model.members["level"].ty.ty_primitive.u32;
        
        var hexString = BitConverter.ToString(owner.data.ToArray()).Replace("-", "").ToLower();

        Debug.Log("owner: " + hexString + "\n"
                  + "character_id: " + character_id + "\n" + "skin_id: " + skin_id + "\n" + "owned: " + owned
                  + "\n" + "level: " + level);
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
