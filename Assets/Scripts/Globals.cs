using Assets.Scripts.DataModels;
using Assets.Scripts.Objects;

using Newtonsoft.Json;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    
    internal static class Globals
    {
        #region PlayerStats

        public static Player Player = new Player();

        #endregion

        #region WorldGen Config
        public static int lastChunks = 15;

        public static int planetID = 0;
        #endregion

        public static Dictionary<string, string> TileResourceMap = new Dictionary<string, string>();
        public static Dictionary<string, Resource> ResourceDictionary;
        public static List<Planet> PlanetList;
        public static List<Faction> FactionList;
        public static List<Item> ItemList;

        private static bool loaded = false;

        public static void LoadEverything()
        {
            if (loaded)
                return;
            loaded = true;

            ResourceDictionary = LoadAllResources();
            PlanetList = LoadAllPlanets();
            FactionList = LoadAllFactions();
            ItemList = LoadAllItems();

            Debug.Log("everytingLoaded");
        }
        public static Dictionary<string, Resource> LoadAllResources()
        {
            var resouceDataList = new List<ResourceDataModel>();
            var jsonFiles = Resources.LoadAll<TextAsset>("JSON/Resources");

            foreach (var file in jsonFiles)
            {
                var wrapper = JsonConvert.DeserializeObject<ResourceDataList>(file.text);
                if (wrapper?.Resources != null)
                    resouceDataList.AddRange(wrapper.Resources);
            }
            var resourceDict = new Dictionary<string, Resource>();
            foreach (ResourceDataModel resourceData in resouceDataList)
            {
                //There will be a global TileName->Resource map Dict<string, string>
                resourceDict[resourceData.Name] = new Resource(resourceData);
                foreach (string tile in resourceDict[resourceData.Name].TileNames)
                {
                    TileResourceMap[tile] = resourceData.Name;
                }
            }
            return resourceDict;
        }

        public static List<Planet> LoadAllPlanets()
        {
            var planetDataList = new List<PlanetConfigDataModel>();
            var jsonFiles = Resources.LoadAll<TextAsset>("JSON/Planets");

            foreach (var file in jsonFiles)
            {
                var wrapper = JsonConvert.DeserializeObject<PlanetList>(file.text);
                if (wrapper?.PlanetListProperty != null)
                    planetDataList.AddRange(wrapper.PlanetListProperty);
            }
            var planetList = new List<Planet>();
            foreach (PlanetConfigDataModel planetData in planetDataList)

                planetList.Add(new Planet(planetData));

            return planetList;
        }

        public static List<Faction> LoadAllFactions()
        {
            var factionDataList = new List<FactionDataModel>();
            var jsonFiles = Resources.LoadAll<TextAsset>("JSON/Items");

            foreach (var file in jsonFiles)
            {
                var wrapper = JsonConvert.DeserializeObject<FactionList>(file.text);
                if (wrapper?.TabList != null)
                    factionDataList.AddRange(wrapper.TabList);
            }

            var factionList = new List<Faction>();
            foreach(FactionDataModel factionData in factionDataList)
                factionList.Add(new Faction(factionData));

            return factionList;
        }

        public static List<Item> LoadAllItems()
        {
            var itemDataList = new List<ItemDataModel>();
            var jsonFiles = Resources.LoadAll<TextAsset>("JSON/Items");

            foreach (var file in jsonFiles)
            {
                var wrapper = JsonConvert.DeserializeObject<ItemList>(file.text);
                if (wrapper?.Items != null)
                    itemDataList.AddRange(wrapper.Items);
            }
            var itemList = new List<Item>();
            foreach (ItemDataModel itemData in itemDataList)
                itemList.Add(new Item(itemData));

            return itemList;
        }
       
    }
}
