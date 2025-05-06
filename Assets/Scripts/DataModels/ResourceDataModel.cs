using System.Collections.Generic;

using Newtonsoft.Json;

namespace Assets.Scripts.DataModels
{
    public class ResourceDataList
    {
        [JsonProperty("Resources")]
        public List<ResourceDataModel> Resources { get; set; }
    }

    public class ResourceDataModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Toughness")]
        public float Toughness { get; set; }

        [JsonProperty("Total")]
        public int Total { get; set; }

        [JsonProperty("TileLocation")]
        public string TileLocation { get; set; }
    }
}
