using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace Assets.Scripts
{
    public class ItemList
    {
        [JsonProperty("Items")]
        public List<ItemDataModel> Items { get; set; }

        public ItemList(List<ItemDataModel> items)
        {
            Items = items;
        }
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

        public ItemDataModel(
            string id,
            string name,
            string description,
            Dictionary<string, int> costs,
            Vector2Int size,
            string faction,
            string research
        )
        {
            Id = id;
            Name = name;
            Description = description;
            Costs = costs;
            Size = size;
            Faction = faction;
            Research = research;
        }
    }
}
