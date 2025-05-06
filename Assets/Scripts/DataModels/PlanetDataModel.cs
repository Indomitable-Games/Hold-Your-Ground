using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Assets.Scripts.Objects
{
    [Serializable]
    public class PlanetList
    {
        [JsonProperty("planets")]
        public List<PlanetConfigDataModel> planetList;
    }

    [Serializable]
    public class PlanetConfigDataModel
    {
        [JsonProperty("seed")]
        public int seed;

        [JsonProperty("veinHeight")]
        public float veinHeight;

        [JsonProperty("rockVariantNames")]
        public List<string> rockVariantNames;

        [JsonProperty("ores")]
        public List<OreConfig> ores;

        [JsonProperty("battleDepths")]
        public List<int> battleDepths;

        [JsonProperty("frequency")]
        public float frequency;

        [JsonProperty("noiseType")]
        public FastNoiseLite.NoiseType noiseType;

        [JsonProperty("fractalType")]
        public FastNoiseLite.FractalType fractalType;

        [JsonProperty("octaves")]
        public int octaves;

        [JsonProperty("lacunarity")]
        public float lacunarity;

        [JsonProperty("gain")]
        public float gain;

        [JsonProperty("weightedStrength")]
        public float weightedStrength;

        [JsonProperty("pingPongStrength")]
        public float pingPongStrength;

        [JsonProperty("cellularDistanceFunction")]
        public FastNoiseLite.CellularDistanceFunction cellularDistanceFunction;

        [JsonProperty("cellularReturnType")]
        public FastNoiseLite.CellularReturnType cellularReturnType;

        [JsonProperty("cellularJitterModifier")]
        public float cellularJitterModifier;

        [JsonProperty("domainWarpType")]
        public FastNoiseLite.DomainWarpType domainWarpType;

        [JsonProperty("domainWarpAmp")]
        public float domainWarpAmp;
    }

    [Serializable]
    public class OreConfig
    {
        [JsonProperty("resourceName")]
        public string resourceName;

        [JsonProperty("spawnChance")]
        public float spawnChance;
    }
}
