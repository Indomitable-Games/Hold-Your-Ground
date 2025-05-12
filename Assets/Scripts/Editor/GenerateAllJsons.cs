using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Assets.Scripts;
using Newtonsoft.Json;

public static class GenerateAllJsons
{
    [MenuItem("Tools/Generate All Sample Jsons")]
    public static void GenerateSampleFactionJson()
    {
        FactionJsonGenerator.GenerateSampleFactionJson();
        ItemJsonGenerator.GenerateSampleItemJson();
        PlanetJsonGenerator.GenerateSamplePlanetJson();
        ResourceJsonGenerator.GenerateSampleResourceJson();
    }
}
