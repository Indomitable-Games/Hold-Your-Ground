using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Assets.Scripts.Objects;

public static class PlanetJsonGenerator
{
    [MenuItem("Tools/Generate Sample Planet JSON")]
    public static void GenerateSamplePlanetJson()
    {
        var planetList = new PlanetList
        (
            new List<PlanetConfigDataModel>
            {
                new PlanetConfigDataModel(
                    new List<OreConfig>
                    {
                        new OreConfig("Stone",1),
                        new OreConfig("Iron", 0.5f),
                        new OreConfig("Gold", 0.3f),
                        new OreConfig("Copper", 0.2f)
                    },
                    42,
                    0.65f,
                    new List<int> { -10000, -20000, -30000 },
                    0.015f,
                    FastNoiseLite.NoiseType.OpenSimplex2,
                    FastNoiseLite.FractalType.FBm,
                    5,
                    2.0f,
                    0.5f,
                    0.8f,
                    1.5f,
                    FastNoiseLite.CellularDistanceFunction.Euclidean,
                    FastNoiseLite.CellularReturnType.CellValue,
                    0.45f,
                    FastNoiseLite.DomainWarpType.OpenSimplex2,
                    20.0f
                )
            }
        );

        string json = JsonConvert.SerializeObject(planetList, Formatting.Indented);
        string outputPath = "Assets/Resources/JSON/Planets/sample_planets.json";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, json);

        AssetDatabase.Refresh();
        Debug.Log("Sample planet JSON generated at: " + outputPath);
    }
}
