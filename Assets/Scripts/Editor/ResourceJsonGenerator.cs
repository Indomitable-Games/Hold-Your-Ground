using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.DataModels;
using Newtonsoft.Json;

public static class ResourceJsonGenerator
{
    [MenuItem("Tools/Generate Sample Resource JSON")]
    public static void GenerateSampleJson()
    {
        var resources = new List<ResourceDataModel>
        {
            new(
                "Stone",
                "test again",
                1.2f,
                new List<string>
                {
                    "Tile/TilePalette/BaseTiles/Base_Stone_14",
                    "Tile/TilePalette/BaseTiles/Base_Stone_0",
                    "Tile/TilePalette/BaseTiles/Base_Stone_7"
                },
                .5f
            ),
            new(
                "Coal",
                "test agains",
                0.2f,
                new List<string>
                {
                    "Tile/TilePalette/OreTiles/Coal-1",
                    "Tile/TilePalette/OreTiles/Coal-2"
                },
                .5f
            ),
            new(
                "Copper",
                "test agains",
                0.2f,
                new List<string>
                {
                    "Tile/TilePalette/OreTiles/Copper-1",
                    "Tile/TilePalette/OreTiles/Copper-2"
                },
                .5f
            ),
            new(
                "Diamond",
                "test agains",
                0.2f,
                new List<string>
                {
                    "Tile/TilePalette/OreTiles/Diamond-1",
                    "Tile/TilePalette/OreTiles/Diamond-2"
                },
                .5f
            ),
            new(
                "Gold",
                "test agains",
                0.2f,
                new List<string>
                {
                    "Tile/TilePalette/OreTiles/Gold-1",
                    "Tile/TilePalette/OreTiles/Gold-2"
                },
                .5f
            ),
            new(
                "Iron",
                "test agains",
                0.2f,
                new List<string>
                {
                    "Tile/TilePalette/OreTiles/Iron-1",
                    "Tile/TilePalette/OreTiles/Iron-2"
                },
                .5f
            ),
            new(
                "Lapis",
                "test agains",
                0.2f,
                new List<string>
                {
                    "Tile/TilePalette/OreTiles/Lapis-1",
                    "Tile/TilePalette/OreTiles/Lapis-2"
                },
                .5f
            )
        };

        var resourceList = new ResourceDataList(resources);

        string json = JsonConvert.SerializeObject(resourceList, Formatting.Indented);
        string outputPath = "Assets/Resources/JSON/Resources/sample_resources.json";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, json);

        AssetDatabase.Refresh();
        Debug.Log("Sample resource JSON generated at: " + outputPath);
    }
}
