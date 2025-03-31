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
        public Sprite Image { get; set; }
        public Resource(string name, string description, float toughness, Sprite image = null)
        {
            Name = name;
            Description = description;
            Toughness = toughness;
            if(image != null)
                Image = image;
            else
                try
                {
                    Image = (Resources.Load("T") as Tile).sprite;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Fucky shit is happening.");
                    throw ex;
                }
                Total = 0;
        }

    }
}
