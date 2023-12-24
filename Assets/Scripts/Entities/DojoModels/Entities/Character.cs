using System;
using Dojo;
using Dojo.Torii;
using UnityEngine;

namespace Amegakure.Starkane.Entities
{
    public enum CharacterType
    {
        None,
        Archer,
        Cleric,
        Warrior,
        Pig,
        Peasant
    }

    public class Character : ModelInstance
    {
        private CharacterType character_id;
        // private CharacterType character_type;
        private UInt64 hp;
        private UInt64 mp;
        private UInt64 attack;
        private UInt64 defense;
        private UInt64 evasion;
        private UInt64 crit_chance;
        private UInt64 crit_rate;
        private UInt64 movement_range;

        public override void Initialize(Model model)
        {
            character_id = (CharacterType)model.members["character_id"].ty.ty_primitive.u32;
            hp = model.members["hp"].ty.ty_primitive.u64;
            mp = model.members["mp"].ty.ty_primitive.u64;
            attack = model.members["attack"].ty.ty_primitive.u64;
            defense = model.members["defense"].ty.ty_primitive.u64;
            evasion = model.members["evasion"].ty.ty_primitive.u64;
            crit_chance = model.members["crit_chance"].ty.ty_primitive.u64;
            movement_range = model.members["movement_range"].ty.ty_primitive.u64;

            Debug.Log("Character: " + character_id + "\n"
                      + "HP: " + hp + "\n" + "attack: " + attack + "\n" + "defense: " + defense
                      + "\n" + "evasion: " + evasion + "\n" + "crit_chance: " + crit_chance
                      + "\n" + "movement_range: " + movement_range);
        }

    }
}
