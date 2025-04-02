using static FastNoiseLite;
using System.Collections.Generic;
using System;
using Assets.Scripts.Objects;
using System.Drawing;
using TMPro.EditorUtilities;
using System.Linq;

namespace Assets.Scripts.Objects
{
    public struct Ore
    {
        public Resource resource;
        public float spawnChance;

        public Ore(Resource resource, float spawnChance)
        {
            this.resource = resource;
            this.spawnChance = spawnChance;
        }
    }

    public class Planet
    {
        #region World Gen
        private FastNoiseLite noiseGen;
        private FastNoiseLite veinNoiseGen;
        private float veinHeight;
        #endregion

        const int VEIN_OFFSET = 1;
        private int _seed;
        public int Seed
        {
            get => _seed;
            set
            {
                _seed = value;
                noiseGen.SetSeed(value);
                veinNoiseGen.SetSeed(value+VEIN_OFFSET);
            }
        }

        public List<Resource> rockVariants { get; }
        public List<Ore> ores { get; }

        public Planet(
            int seed,
            float mFrequency,
            NoiseType mNoiseType,
            FractalType mFractalType,
            int mOctaves,
            float mLacunarity,
            float mGain,
            float mWeightedStrength,
            float mPingPongStrength,
            CellularDistanceFunction mCellularDistanceFunction,
            CellularReturnType mCellularReturnType,
            float mCellularJitterModifier,
            DomainWarpType mDomainWarpType,
            float mDomainWarpAmp,
            float veinHeight,
            List<Resource> rockVariants,
            List<Ore> ores)
        {
            noiseGen = new FastNoiseLite();
            noiseGen.SetFrequency(mFrequency);
            noiseGen.SetNoiseType(mNoiseType);
            noiseGen.SetFractalType(mFractalType);
            noiseGen.SetFractalOctaves(mOctaves);
            noiseGen.SetFractalLacunarity(mLacunarity);
            noiseGen.SetFractalGain(mGain);
            noiseGen.SetFractalWeightedStrength(mWeightedStrength);
            noiseGen.SetFractalPingPongStrength(mPingPongStrength);
            noiseGen.SetCellularDistanceFunction(mCellularDistanceFunction);
            noiseGen.SetCellularReturnType(mCellularReturnType);
            noiseGen.SetCellularJitter(mCellularJitterModifier);
            noiseGen.SetDomainWarpType(mDomainWarpType);
            noiseGen.SetDomainWarpAmp(mDomainWarpAmp);

            veinNoiseGen = new FastNoiseLite(); // Separate noise for vein mapping
            Seed = seed;
            veinNoiseGen.SetFrequency(mFrequency * 0.5f); // Lower frequency for larger patches
            veinNoiseGen.SetNoiseType(NoiseType.Perlin);
            if (veinHeight <= 0 || veinHeight >= 1)
                throw new ArgumentException("Invalid Vein Height");
            this.veinHeight = veinHeight;
            this.rockVariants = new List<Resource>(rockVariants);
            this.ores = new List<Ore>(ores);
        }

        public float GetNoise(Point point) => noiseGen.GetNoise(point.X, point.Y);
        public float GetVeinNoise(Point point) => veinNoiseGen.GetNoise(point.X, point.Y);

        public Resource GetResource(Point point)
        {
            float noiseValue = (GetNoise(point)+1)/2;

            if (noiseValue < veinHeight)
            {
                return rockVariants[Math.Abs((int)(GetVeinNoise(point) * rockVariants.Count)) % rockVariants.Count];
            }

            float totalSpawnRate = 0;
            
            totalSpawnRate = ores.Sum(x => x.spawnChance);

            float veinNoiseValue = (GetVeinNoise(point) + 1) / 2; // Normalize to 0-1
            float weightedValue = veinNoiseValue * totalSpawnRate;
            float cumulative = 0;
            foreach (var ore in ores)
            {
                cumulative += ore.spawnChance;
                if (weightedValue <= cumulative)
                {
                    return ore.resource;
                }
            }
            return ores[^1].resource; // Default to the last ore in case of rounding issues
        }
    }
}
