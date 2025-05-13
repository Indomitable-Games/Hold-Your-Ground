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
        public float Speed = 20f;
        public float TurnRadius = 60f;
        public float TurnSpeed = .4f;

        public int Fuel = 5000;

        public Dictionary<Resource, int> PlayerResources = new Dictionary<Resource, int>();
    }
}