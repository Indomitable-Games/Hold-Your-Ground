using Assets.Scripts.DataModels;

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
        public List<string> TileList { get; set; }
        public bool IsBaseTile { get; set; }
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

        public Resource(Resource other)
        {
            Name = other.Name;
            TileName = other.TileName;
            Description = other.Description;
            Toughness = other.Toughness;
            Total = other.Total;

            // Tile is a reference type — clone if needed, otherwise just copy reference
            tile = other.tile != null ? other.tile : null;
        }

        public Resource(ResourceDataModel model)
        {
            Name = model.Name;
            TileName = model.TileLocation.Split('/')[^1];
            Description = model.Description;
            Toughness = model.Toughness;
            Total = model.Total;

            tile = (Resources.Load(model.TileLocation) as Tile);

        }
    }
}
