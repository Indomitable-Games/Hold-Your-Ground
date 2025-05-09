using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Assets.Scripts;
using Newtonsoft.Json;

public static class FactionJsonGenerator
{
    [MenuItem("Tools/Generate Sample Faction JSON")]
    public static void GenerateSampleJson()
    {
        var factionList = new FactionList(new List<FactionDataModel>
        {
            new(
                "Dwarves",
                "Stout miners of the deep.",
                new List<ResearchDataModel>
                {
                    new("Deep Mining", "Unlock advanced mining tech.",
                        new Dictionary<string, int> { { "Iron", 100 }, { "Gold", 50 } },
                        new List<string>(), new List<string> { "Explosive Mining" }),
                    new("Explosive Mining", "Use controlled blasts to mine faster.",
                        new Dictionary<string, int> { { "Iron", 150 }, { "Gunpowder", 75 } },
                        new List<string> { "Deep Mining" }, new List<string>())
                },
                "icon_dwarf.png"
            )
        });

        string json = JsonConvert.SerializeObject(factionList, Formatting.Indented);
        string outputPath = "Assets/Resources/JSON/Factions/sample_factions.json";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, json);

        AssetDatabase.Refresh();
        Debug.Log("Sample faction JSON generated at: " + outputPath);
    }
}
