using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Objects
{
    public class Resource
    {
        //change type to Enum  vvvvv
        public string Name { get; set; }
        public string TileName { get; set; }
        public string Description { get; set; }
        public float Toughness { get; set; }
        public int Total { get; set; }
        public Tile tile { get; set; }
        public Resource(string name, string description, float toughness, Tile tile = null)
        {
            Name = name;
            Description = description;
            Toughness = toughness;
            if(tile != null)
                this.tile = tile;
            else
                try
                {
                    Debug.LogWarning($"kinda fucky shit is happening with {name}.");
                    tile = (Resources.Load("T") as Tile);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"really Fucky shit is happening with {name}.");
                    throw ex;
                }
            if (tile == null)
            {
                Debug.LogError($"this shouldnt even be possible with {name}.");
            }
                Total = 0;
        }

    }
}
