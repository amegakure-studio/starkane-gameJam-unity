using Dojo;
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
    [ModelField("id")]
    public UInt32 id;

    [ModelField("character_id")]
    public UInt32  character_id;

    [ModelField("level")]
    public byte level;

    [ModelField("character_level_required")]
    public byte character_level_required;

    [ModelField("skill_type")]
    public byte skill_type;

    [ModelField("power")]
    public UInt64  power;

    [ModelField("mp_cost")]
    public UInt64 mp_cost;

    [ModelField("range")]
    public UInt64 range;

  
    public uint Id { get => id; set => id = value; }
    public uint Character_id { get => character_id; set => character_id = value; }
    public byte Level { get => level; set => level = value; }
    public byte Character_level_required { get => character_level_required; set => character_level_required = value; }
    public SkillType Skill_type { get => (SkillType)skill_type; }
    public ulong Power { get => power; set => power = value; }
    public ulong Mp_cost { get => mp_cost; set => mp_cost = value; }
    public ulong Range { get => range; set => range = value; }
    public bool IsSpecial()
    { return Skill_type != SkillType.MeleeAttack && Skill_type != SkillType.RangeAttack; }

    private Pathfinder m_Pathfinder;

    private void Awake()
    {
        m_Pathfinder = new();
    }

    public Frontier GetFrontier(Tile origin)
    {
        if (origin != null)
        {
            Frontier frontier = m_Pathfinder.FindPaths(origin, (int)range, PathStyle.CROSS, false);
            return frontier;
        }

        else
            return new Frontier();
    }
}

