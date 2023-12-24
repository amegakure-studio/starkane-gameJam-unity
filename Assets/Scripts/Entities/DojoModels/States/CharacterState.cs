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

        public override void Initialize(Model model)
        {
            match_id = model.members["match_id"].ty.ty_primitive.u32;
            character_id = model.members["character_id"].ty.ty_primitive.u32;
            player = model.members["player"].ty.ty_primitive.felt252;
            remain_hp = model.members["remain_hp"].ty.ty_primitive.u64;
            remain_mp = model.members["remain_mp"].ty.ty_primitive.u64;
            x = model.members["x"].ty.ty_primitive.u64;
            y = model.members["y"].ty.ty_primitive.u64;
            
            var hexString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();

            Debug.Log("match_id: " + match_id + "\n"
                    + "character_id: " + character_id + "\n" +
                    "player: "+ hexString + "\n"
                    + "remain_hp: " + remain_hp + "\n" 
                    + "remain_mp: " + remain_mp + "\n"
                    + "X:" + x + "\n"
                    + "Y:" + y + "\n" );
        }
    }

}
