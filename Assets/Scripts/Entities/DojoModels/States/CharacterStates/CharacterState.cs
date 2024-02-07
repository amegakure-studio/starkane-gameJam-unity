using System;
using System.Numerics;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
namespace Amegakure.Starkane.Entities
{
    public class CharacterState : ModelInstance
    {
        [ModelField("match_id")]
        private UInt32 match_id;

        [ModelField("character_id")]
        private UInt32 character_id;

        [ModelField("player")]
        dojo.FieldElement player;

        [ModelField("remain_hp")]
        private UInt64 remain_hp;

        [ModelField("remain_mp")]
        private UInt64 remain_mp;

        [ModelField("x")]
        private UInt64 x;

        [ModelField("y")]
        private UInt64 y;

        private BigInteger player_id;

        public event Action<CharacterState> OnDead;
        public uint Match_id { get => match_id; set => match_id = value; }
        public uint Character_id { get => character_id; set => character_id = value; }
        public dojo.FieldElement Player { get => player; set => player = value; }
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
        public BigInteger Player_id{ get => player_id; set => player_id = value; }

        public override void Initialize(Model model)
        {
            base.Initialize(model);

            var hexString = BitConverter.ToString(Player.data.ToArray()).Replace("-", "").ToLower();
            player_id = BigInteger.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public override void OnUpdate(Model model)
        {
            base.OnUpdate(model);

            if (Remain_hp <= 0)
                OnDead?.Invoke(this);
        }
    }

}
