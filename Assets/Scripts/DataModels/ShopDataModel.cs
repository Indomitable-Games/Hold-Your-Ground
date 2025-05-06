using System.Collections.Generic;

using Newtonsoft.Json;

namespace Assets.Scripts
{
    public class ShopTabList
    {
        [JsonProperty("Tabs")]
        public List<ShopTab> TabList { get; set; }
    }

    public class ShopTab
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("FactionFilter")]
        public Faction FactionFilter { get; set; }
    }
}
