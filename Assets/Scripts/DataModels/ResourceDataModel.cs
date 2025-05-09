using System.Collections.Generic;

using Newtonsoft.Json;

namespace Assets.Scripts.DataModels
{
    public class ResourceDataList
    {
        [JsonProperty("resources")]
        public List<ResourceDataModel> Resources { get; set; }

        public ResourceDataList(List<ResourceDataModel> resources)
        {
            Resources = resources;
        }
    }

    public class ResourceDataModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("toughness")]
        public float Toughness { get; set; }



        [JsonProperty("tileLocations")]
        public List<string> TileLocations { get; set; }


        [JsonProperty("mainTileChance")]
        public float MainTileChance { get; set; }

        public ResourceDataModel(string name, string description, float toughness, List<string> tileLocations, float mainTileChance)  
        {
            Name = name;
            Description = description;
            Toughness = toughness;
            TileLocations = tileLocations;
            MainTileChance = mainTileChance;
        }
    }
}
