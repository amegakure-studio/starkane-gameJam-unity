using Amegakure.Starkane.Entities;
using System;

namespace Amegakure.Starkane.Id
{
    public class CharacterId
    {
        private CharacterType characterType;
        private int playerId;

        public CharacterId (int _playerId, CharacterType _characterType)
        {
            characterType = _characterType;
            playerId = _playerId;
        }

        public CharacterType CharacterType { get => characterType; private set => characterType = value; }
        public int PlayerId { get => playerId; private set => playerId = value; }

        public override bool Equals(object obj)
        {
            return obj is CharacterId id &&
                   characterType == id.characterType &&
                   playerId == id.playerId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(characterType, playerId);
        }
    }
}
