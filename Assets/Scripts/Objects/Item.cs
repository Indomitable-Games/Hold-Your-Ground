using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Scripts.Objects
{
    internal class Item
    {
        private ItemDataModel itemData;

        public string Name { get; set; }
        public string Description { get; set; }
        public string faction { get; set; }
        public Item(ItemDataModel itemData)
        {
            this.itemData = itemData;
            this.Name = itemData.Name;
            this.Description = itemData.Description;
            faction = itemData.Faction;

            if (Globals.FactionList.Any(x => x.Name == faction))
                Debug.LogError($"Item faction miss match {Name}");
        }
    }
}
