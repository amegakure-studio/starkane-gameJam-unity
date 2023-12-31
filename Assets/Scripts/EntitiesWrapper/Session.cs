using Amegakure.Starkane.EntitiesWrapper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amegakure.Starkane.EntitiesWrapper
{
    public class Session : MonoBehaviour
    {
        [SerializeField] Player player;

        public Player Player { get => player; set => player = value; }

    }
}
