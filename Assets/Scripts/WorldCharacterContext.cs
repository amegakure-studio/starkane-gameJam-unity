using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.Id;
using UnityEngine;

namespace Amegakure.Starkane.Context
{
    public class WorldCharacterContext : MonoBehaviour
    {
        private CharacterId id;
        private Tile location;

        public CharacterId Id { get => id; set => id = value; }
        public Tile Location { get => location; set => location = value; }
    }

}
