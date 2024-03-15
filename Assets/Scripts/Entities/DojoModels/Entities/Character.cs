using System;
using Dojo;

namespace Amegakure.Starkane.Entities
{
    public enum CharacterType
    {
        None,
        Archer,
        Cleric,
        Warrior,
        Goblin,
        Peasant,
        Goblin2,
    }

    public class Character : ModelInstance
    {
        [ModelField("hp")]
        public UInt64 hp;

        [ModelField("mp")]
        public UInt64 mp;

        [ModelField("attack")]
        public UInt32 attack;

        [ModelField("defense")]
        public UInt32 defense;

        [ModelField("evasion")]
        public UInt32 evasion;

        [ModelField("crit_chance")]
        public UInt32 crit_chance;

        [ModelField("crit_rate")]
        public UInt32 crit_rate;

        [ModelField("movement_range")]
        public UInt32 movement_range;

        [ModelField("character_id")]
        public CharacterType character_id;
    }
}
