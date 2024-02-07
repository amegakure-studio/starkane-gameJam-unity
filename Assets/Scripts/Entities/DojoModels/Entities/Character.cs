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
        private UInt64 hp;

        [ModelField("mp")]
        private UInt64 mp;

        [ModelField("attack")]
        private UInt64 attack;

        [ModelField("defense")]
        private UInt64 defense;

        [ModelField("evasion")]
        private UInt64 evasion;

        [ModelField("crit_chance")]
        private UInt64 crit_chance;

        [ModelField("crit_rate")]
        private UInt64 crit_rate;

        [ModelField("movement_range")]
        private UInt64 movement_range;

        private CharacterType character_id;

        public int Hp { get => (int)hp; }
        public int Mp { get => (int)mp; }
        public int Attack { get => (int)attack;  }
        public int Defense { get => (int)defense; }
        public int Evasion { get => (int)evasion; }
        public int Crit_chance { get => (int)crit_chance; }
        public int Crit_rate { get => (int)crit_rate; }
        public int Movement_range { get => (int)movement_range; }
        public int Character_id { get => (int) character_id; }

    }
}
