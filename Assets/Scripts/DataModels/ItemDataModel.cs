using System.Collections.Generic;

using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts
{

    public class ItemList
    {
        [JsonProperty("Items")]
        public List<ItemDataModel> Items { get; set; }
    }

    public class ItemDataModel
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Costs")]
        public Dictionary<string, int> Costs { get; set; }

        [JsonProperty("Size")]
        public Vector2Int Size { get; set; }

        [JsonProperty("Faction")]
        public string Faction { get; set; }

        [JsonProperty("Research")]
        public string Research { get; set; }

        /*
         * Activatable
         * range
         * uses
         * persistant (stays after mission)
         * deployable (crew members)
         * requires crew member (turrets that need to be build)
         * has ammo (drill launcher)
         * is ammo
         * ammo count
         */
    }
}
