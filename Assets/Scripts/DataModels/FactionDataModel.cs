using System.Collections.Generic;

using Newtonsoft.Json;

namespace Assets.Scripts
{
    public class FactionList
    {
        [JsonProperty("factions")]
        public List<FactionDataModel> TabList { get; set; }
    }

    public class FactionDataModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("research")]
        public List<ResearchDataModel> ResearchList { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public class ResearchDataModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("costs")]
        public Dictionary<string, int> Costs { get; set; }

        [JsonProperty("parents")]
        public List<string> Prarent { get; set; }

        [JsonProperty("children")]
        public List<string> Child { get; set; }
    }
}
