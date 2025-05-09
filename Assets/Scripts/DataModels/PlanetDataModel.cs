using System;
using System.Collections.Generic;

using Assets.Scripts.DataModels;

using Newtonsoft.Json;

namespace Assets.Scripts.Objects
{
    [Serializable]
    public class PlanetList
    {
        [JsonProperty("planets")]
        public List<PlanetConfigDataModel> PlanetListProperty;

        public PlanetList(List<PlanetConfigDataModel> planets)
        {
            PlanetListProperty = planets;
        }
    }

    [Serializable]
    public class PlanetConfigDataModel
    {
        [JsonProperty("tiles")]
        public List<OreConfig> Tiles { get; set; }

        [JsonProperty("seed")]
        public int Seed { get; set; }

        [JsonProperty("veinHeight")]
        public float VeinHeight { get; set; }

        [JsonProperty("rockVariantNames")]
        public List<string> RockVariantNames { get; set; }

        [JsonProperty("battleDepths")]
        public List<int> BattleDepths { get; set; }

        [JsonProperty("frequency")]
        public float Frequency { get; set; }

        [JsonProperty("noiseType")]
        public FastNoiseLite.NoiseType NoiseType { get; set; }

        [JsonProperty("fractalType")]
        public FastNoiseLite.FractalType FractalType { get; set; }

        [JsonProperty("octaves")]
        public int Octaves { get; set; }

        [JsonProperty("lacunarity")]
        public float Lacunarity { get; set; }

        [JsonProperty("gain")]
        public float Gain { get; set; }

        [JsonProperty("weightedStrength")]
        public float WeightedStrength { get; set; }

        [JsonProperty("pingPongStrength")]
        public float PingPongStrength { get; set; }

        [JsonProperty("cellularDistanceFunction")]
        public FastNoiseLite.CellularDistanceFunction CellularDistanceFunction { get; set; }

        [JsonProperty("cellularReturnType")]
        public FastNoiseLite.CellularReturnType CellularReturnType { get; set; }

        [JsonProperty("cellularJitterModifier")]
        public float CellularJitterModifier { get; set; }

        [JsonProperty("domainWarpType")]
        public FastNoiseLite.DomainWarpType DomainWarpType { get; set; }

        [JsonProperty("domainWarpAmp")]
        public float DomainWarpAmp { get; set; }

        // Constructor
        public PlanetConfigDataModel(
            List<OreConfig> tiles,
            int seed,
            float veinHeight,
            List<int> battleDepths,
            float frequency,
            FastNoiseLite.NoiseType noiseType,
            FastNoiseLite.FractalType fractalType,
            int octaves,
            float lacunarity,
            float gain,
            float weightedStrength,
            float pingPongStrength,
            FastNoiseLite.CellularDistanceFunction cellularDistanceFunction,
            FastNoiseLite.CellularReturnType cellularReturnType,
            float cellularJitterModifier,
            FastNoiseLite.DomainWarpType domainWarpType,
            float domainWarpAmp)
        {
            Tiles = tiles;
            Seed = seed;
            VeinHeight = veinHeight;
            BattleDepths = battleDepths;
            Frequency = frequency;
            NoiseType = noiseType;
            FractalType = fractalType;
            Octaves = octaves;
            Lacunarity = lacunarity;
            Gain = gain;
            WeightedStrength = weightedStrength;
            PingPongStrength = pingPongStrength;
            CellularDistanceFunction = cellularDistanceFunction;
            CellularReturnType = cellularReturnType;
            CellularJitterModifier = cellularJitterModifier;
            DomainWarpType = domainWarpType;
            DomainWarpAmp = domainWarpAmp;
        }
    }

    [Serializable]
    public class OreConfig
    {
        [JsonProperty("resourceName")]
        public string ResourceName { get; set; }

        [JsonProperty("spawnChance")]
        public float SpawnChance { get; set; }

        public OreConfig(string resourceName, float spawnChance)
        {
            ResourceName = resourceName;
            SpawnChance = spawnChance;
        }
    }
}
