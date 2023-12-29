using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using System;
using Amegakure.Starkane.GridSystem;

public enum SkillType
{
    None,
    MeleeAttack,
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

    public uint Id { get => id; set => id = value; }
    public uint Character_id { get => character_id; set => character_id = value; }
    public byte Level { get => level; set => level = value; }
    public byte Character_level_required { get => character_level_required; set => character_level_required = value; }
    public SkillType Skill_type { get => (SkillType)skill_type; }
    public ulong Power { get => power; set => power = value; }
    public ulong Mp_cost { get => mp_cost; set => mp_cost = value; }
    public ulong Range { get => range; set => range = value; }

    private Pathfinder m_Pathfinder;

    private void Awake()
    {
        m_Pathfinder = new();
    }


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

    public Frontier GetFrontier(Tile origin)
    {
        if (origin != null)
        {
            Frontier frontier = m_Pathfinder.FindPaths(origin, (int)range, PathStyle.SQUARE, false);
            return frontier;
        }

        else
            return new Frontier();
    }
}

