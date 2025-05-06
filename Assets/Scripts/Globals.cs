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

        public static float playerSpeed = 20f;
        public static float playerTurnRadius = 30f;
        public static float playerTurnSpeed = .4f;

        public static int fuel = 5000;

        public static Resource[] playerResources = {
            new Resource("R1", "Soft stuff", 0.1f, Resources.Load("Tile\\TilePalette\\BaseTiles\\Base_Stone_0") as Tile)
        };

        #endregion

        #region WorldGen Config
        public static int lastChunks = 3;

        public static int planetID = 0;
        public static int currentBattle = 0;
        #endregion

        public static Dictionary<string, Resource> ResourceDictionary;
        public static List<Planet> PlanetList;
        public static List<ShopTab> ShopTabList;
        public static List<Item> ItemList;

        public static void LoadEverything()
        {
            ResourceDictionary = LoadAllResources();
            PlanetList = LoadAllPlanets();
            ShopTabList = LoadAllShopTabs();
            ItemList = LoadAllItems();

            Debug.Log("everytingLoaded");
        }
        public static Dictionary<string, Resource> LoadAllResources()
        {
            var resouceDataList = new List<ResourceDataModel>();
            var jsonFiles = Resources.LoadAll<TextAsset>("JSON/Resource");

            foreach (var file in jsonFiles)
            {
                var wrapper = JsonConvert.DeserializeObject<ResourceDataList>(file.text);
                if (wrapper?.Resources != null)
                    resouceDataList.AddRange(wrapper.Resources);
            }
            var resourceDict = new Dictionary<string, Resource>();
            foreach (ResourceDataModel resouseData in resouceDataList)
                resourceDict[resouseData.Name] = new Resource(resouseData);

            return resourceDict;
        }

        public static List<Planet> LoadAllPlanets()
        {
            var planetDataList = new List<PlanetConfigDataModel>();
            var jsonFiles = Resources.LoadAll<TextAsset>("JSON/Planets");

            foreach (var file in jsonFiles)
            {
                var wrapper = JsonConvert.DeserializeObject<PlanetList>(file.text);
                if (wrapper?.planetList != null)
                    planetDataList.AddRange(wrapper.planetList);
            }
            var planetList = new List<Planet>();
            foreach (PlanetConfigDataModel planetData in planetDataList)

                planetList.Add(new Planet(planetData));

            return planetList;
        }

        public static List<ShopTab> LoadAllShopTabs()
        {
            var itemList = new List<ShopTab>();
            var jsonFiles = Resources.LoadAll<TextAsset>("JSON/Items");

            foreach (var file in jsonFiles)
            {
                var wrapper = JsonConvert.DeserializeObject<ShopTabList>(file.text);
                if (wrapper?.TabList != null)
                    itemList.AddRange(wrapper.TabList);
            }

            return itemList;
        }

        public static List<Item> LoadAllItems()
        {
            var itemList = new List<Item>();
            var jsonFiles = Resources.LoadAll<TextAsset>("JSON/ShopTabs");

            foreach (var file in jsonFiles)
            {
                var wrapper = JsonConvert.DeserializeObject<ItemList>(file.text);
                if (wrapper?.Items != null)
                    itemList.AddRange(wrapper.Items);
            }

            return itemList;
        }
       
    }
}
