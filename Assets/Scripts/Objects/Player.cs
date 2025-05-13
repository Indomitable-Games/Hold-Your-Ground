using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Objects
{
    public class Player
    {
        public float Speed = 10f;
        public float TurnRadius = 75f;
        public float TurnSpeed = .8f;

        public int Fuel = 5000;

        public Dictionary<Resource, int> PlayerResources = new Dictionary<Resource, int>();
    }
}