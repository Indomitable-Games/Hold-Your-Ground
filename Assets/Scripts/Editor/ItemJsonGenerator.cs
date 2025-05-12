using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Assets.Scripts;

public static class ItemJsonGenerator
{
    [MenuItem("Tools/Generate Sample Item JSON")]
    public static void GenerateSampleItemJson()
    {
        var items = new List<ItemDataModel>
        {
            new(
                "stone_drill",
                "Stone Drill",
                "Basic mining drill for extracting ores.",
                new Dictionary<string, int>
                {
                    { "Stone", 20 },
                    { "Iron", 10 }
                },
                new Vector2Int(2, 2),
                "Engineer",
                "drill_tech_1"
            ),
            new(
                "coal_generator",
                "Coal Generator",
                "Generates electricity from coal.",
                new Dictionary<string, int>
                {
                    { "Coal", 15 },
                    { "Iron", 25 }
                },
                new Vector2Int(3, 2),
                "Engineer",
                "power_tech_1"
            ),
            new(
                "barracks",
                "Barracks",
                "Trains and houses troops.",
                new Dictionary<string, int>
                {
                    { "Stone", 30 },
                    { "Wood", 20 }
                },
                new Vector2Int(4, 3),
                "Military",
                "barracks_tech"
            )
        };

        var itemList = new ItemList(items);

        string json = JsonConvert.SerializeObject(itemList, Formatting.Indented);
        string outputPath = "Assets/Resources/JSON/Items/sample_items.json";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, json);

        AssetDatabase.Refresh();
        Debug.Log("Sample item JSON generated at: " + outputPath);
    }
}
