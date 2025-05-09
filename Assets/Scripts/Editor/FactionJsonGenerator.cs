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
        var factionList = new FactionList
        {
            TabList = new List<FactionDataModel>
            {
                new FactionDataModel
                {
                    Name = "Dwarves",
                    Description = "Stout miners of the deep.",
                    Icon = "icon_dwarf.png",
                    ResearchList = new List<ResearchDataModel>
                    {
                        new ResearchDataModel
                        {
                            Name = "Deep Mining",
                            Description = "Unlock advanced mining tech.",
                            Costs = new Dictionary<string, int>
                            {
                                { "Iron", 100 },
                                { "Gold", 50 }
                            },
                            Prarent = new List<string>(), // no parents
                            Child = new List<string> { "Explosive Mining" }
                        },
                        new ResearchDataModel
                        {
                            Name = "Explosive Mining",
                            Description = "Use controlled blasts to mine faster.",
                            Costs = new Dictionary<string, int>
                            {
                                { "Iron", 150 },
                                { "Gunpowder", 75 }
                            },
                            Prarent = new List<string> { "Deep Mining" },
                            Child = new List<string>()
                        }
                    }
                }
            }
        };

        string json = JsonConvert.SerializeObject(factionList, Formatting.Indented);

        string outputPath = "Assets/Resources/JSON/Factions/sample_factions.json";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, json);

        AssetDatabase.Refresh();
        Debug.Log("Sample faction JSON generated at: " + outputPath);
    }
}
