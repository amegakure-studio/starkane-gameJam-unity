using System;
using System.Collections;
using System.Collections.Generic;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;
namespace Amegakure.Starkane.Entities
{
    public class CharacterState : ModelInstance
    {
        private UInt32 match_id;
        private UInt32 character_id;
        dojo.FieldElement player;
        private UInt64 remain_hp;
        private UInt64 remain_mp;
        private UInt64 x;
        private UInt64 y;
        private int player_id;

        public uint Match_id { get => match_id; set => match_id = value; }
        public uint Character_id { get => character_id; set => character_id = value; }
        public dojo.FieldElement Player { get => player; set => player = value; }
        public ulong Remain_hp { get => remain_hp; set => remain_hp = value; }
        public ulong Remain_mp { get => remain_mp; set => remain_mp = value; }
        public ulong X { get => x; set => x = value; }
        public ulong Y { get => y; set => y = value; }
        public int Player_id{ get => player_id; set => player_id = value; }

        public override void Initialize(Model model)
        {
            Match_id = model.members["match_id"].ty.ty_primitive.u32;
            Character_id = model.members["character_id"].ty.ty_primitive.u32;
            Player = model.members["player"].ty.ty_primitive.felt252;
            Remain_hp = model.members["remain_hp"].ty.ty_primitive.u64;
            Remain_mp = model.members["remain_mp"].ty.ty_primitive.u64;
            X = model.members["x"].ty.ty_primitive.u64;
            Y = model.members["y"].ty.ty_primitive.u64;
            
            var hexString = BitConverter.ToString(Player.data.ToArray()).Replace("-", "").ToLower();
            player_id = System.Int32.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
            Debug.Log("CHARACTER STATE: \n match_id: " + Match_id + "\n"
                    + "character_id: " + Character_id + "\n" +
                    "player: "+ hexString + "\n"
                    + "remain_hp: " + Remain_hp + "\n" 
                    + "remain_mp: " + Remain_mp + "\n"
                    + "X:" + X + "\n"
                    + "Y:" + Y + "\n" );
        }
    }

}
