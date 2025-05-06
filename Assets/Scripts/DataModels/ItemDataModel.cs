using System.Collections.Generic;

using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts
{
    public enum Faction
    {
        Base,
        Dwarves,
        Elves,
        Forerunner
    }

    public class ItemList
    {
        [JsonProperty("Items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Costs")]
        public Dictionary<string, int> Costs { get; set; }

        [JsonProperty("Size")]
        public Vector2Int Size { get; set; }

        [JsonProperty("Faction")]
        public Faction Faction { get; set; }

        [JsonProperty("Unlocked")]
        public bool Unlocked { get; set; }
    }
}
