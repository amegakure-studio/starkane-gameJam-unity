using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using System;

public enum SkillType
{
    MeeleAttack,
    RangeAttack,
    Fireball,
    Heal,
}

public class Skill : ModelInstance
{
    private UInt32 id;
    private UInt32  character_id;
    private byte level;
    private byte character_level_required;
    private byte skill_type;
    private UInt64  power;
    private UInt64 mp_cost;
    private UInt64 range;

    public override void Initialize(Model model)
    {
        id = model.members["id"].ty.ty_primitive.u32;
        character_id = model.members["character_id"].ty.ty_primitive.u32;
        level = model.members["level"].ty.ty_primitive.u8;
        character_level_required = model.members["character_level_required"].ty.ty_primitive.u8;
        skill_type = model.members["skill_type"].ty.ty_primitive.u8;
        power = model.members["power"].ty.ty_primitive.u64;
        mp_cost = model.members["mp_cost"].ty.ty_primitive.u64;
        range = model.members["range"].ty.ty_primitive.u64;   

        // Debug.Log(ToString());
    }

    public override string ToString()
    {
        return "id: " + id + "\n"
                  + "character_id: " + character_id + "\n" +
                  "level: " + level + "\n"
                  + "character_level_required: " + character_level_required
                  + "\n" + "skill_type: " + skill_type
                  + "\n" + "power: " + power
                  + "\n" + "mp_cost: " + mp_cost
                  + "\n" + "range: " + range;
    }
}

