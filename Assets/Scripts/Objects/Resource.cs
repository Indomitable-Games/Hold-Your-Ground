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
        public List<string> TileNames { get; set; }
        public string Description { get; set; }
        public float Toughness { get; set; }
        public List<string> TileList { get; set; }
        public bool IsBaseTile { get; set; }
        public List<Tile> Tiles { get; set; }
        public float mainSpawnChance { get; set; }
        
        public Resource(Resource other)
        {
            Name = other.Name;
            TileNames = other.TileNames;
            Description = other.Description;
            Toughness = other.Toughness;
            mainSpawnChance = other.mainSpawnChance;

            // Tile is a reference type — clone if needed, otherwise just copy reference
            Tiles = other.Tiles != null ? other.Tiles : null;
        }

        public Resource(ResourceDataModel model)
        {
            Name = model.Name;
            TileNames = new List<string>();
            Tiles = new List<Tile>();
            foreach (string tileLocation in model.TileLocations)
            {
                TileNames.Add(tileLocation.Split('/')[^1]);
                Tiles.Add(Resources.Load(tileLocation) as Tile);
            }
            Description = model.Description;
            Toughness = model.Toughness;
            mainSpawnChance = model.MainTileChance;
            IsBaseTile = model.IsBase;

        }
    }
}
