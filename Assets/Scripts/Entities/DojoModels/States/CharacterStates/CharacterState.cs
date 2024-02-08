using System;
using System.Diagnostics;
using System.Numerics;
using Dojo;
using Dojo.Starknet;
using Dojo.Torii;
using dojo_bindings;
namespace Amegakure.Starkane.Entities
{
    public class CharacterState : ModelInstance
    {
        [ModelField("match_id")]
        public UInt32 match_id;

        [ModelField("character_id")]
        public UInt32 character_id;

        [ModelField("player")]
        public FieldElement player;

        [ModelField("remain_hp")]
        public UInt64 remain_hp;

        [ModelField("remain_mp")]
        public UInt64 remain_mp;

        [ModelField("x")]
        public UInt64 x;

        [ModelField("y")]
        public UInt64 y;


        public event Action<CharacterState> OnDead;
        public event Action<CharacterState> OnPositionChange;
        public uint Match_id { get => match_id; set => match_id = value; }
        public uint Character_id { get => character_id; set => character_id = value; }
        public FieldElement Player { get => player; set => player = value; }
        public ulong Remain_hp
        {
            get => remain_hp;
            set
            {
                remain_hp = value;
                //if(value <= 0)
                //    OnDead?.Invoke(this);
            }
        }
        public ulong Remain_mp { get => remain_mp; set => remain_mp = value; }
        public ulong X { get => x; set => x = value; }
        public ulong Y { get => y; set => y = value; }

        public override void Initialize(Model model)
        {
            base.Initialize(model);

            //var hexString = BitConverter.ToString(Player.data.ToArray()).Replace("-", "").ToLower();
            //player_id = BigInteger.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public override void OnUpdate(Model model)
        {
            UInt64 oldX = x;
            UInt64 oldY = y;

            base.OnUpdate(model);

            UnityEngine.Debug.Log("Match: " + match_id + "\n"+
                                   "Player: " + player.Hex() + "\n" +
                                    "OLDX: " + oldX +
                                    "OLDX: " + oldY  + "\n" +
                                    "x: " + x +
                                    "y: " + y + "\n");

            if (oldX != x || oldY != y)
                OnPositionChange?.Invoke(this);

            if (Remain_hp <= 0)
                OnDead?.Invoke(this);
        }
    }

}
