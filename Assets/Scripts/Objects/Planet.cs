using static FastNoiseLite;
using System.Collections.Generic;
using System;
using Assets.Scripts.Objects;
using System.Drawing;
using TMPro.EditorUtilities;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        private FastNoiseLite baseRockNoiseGen;
        private FastNoiseLite veinLocationNoiseGen;
        private FastNoiseLite veinTypeNoiseGen;
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
                baseRockNoiseGen.SetSeed(value-VEIN_OFFSET);
                veinLocationNoiseGen.SetSeed(value);
                veinTypeNoiseGen.SetSeed(value+VEIN_OFFSET);
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
            veinLocationNoiseGen = new FastNoiseLite();
            veinLocationNoiseGen.SetFrequency(mFrequency);
            veinLocationNoiseGen.SetNoiseType(mNoiseType);
            veinLocationNoiseGen.SetFractalType(mFractalType);
            veinLocationNoiseGen.SetFractalOctaves(mOctaves);
            veinLocationNoiseGen.SetFractalLacunarity(mLacunarity);
            veinLocationNoiseGen.SetFractalGain(mGain);
            veinLocationNoiseGen.SetFractalWeightedStrength(mWeightedStrength);
            veinLocationNoiseGen.SetFractalPingPongStrength(mPingPongStrength);
            veinLocationNoiseGen.SetCellularDistanceFunction(mCellularDistanceFunction);
            veinLocationNoiseGen.SetCellularReturnType(mCellularReturnType);
            veinLocationNoiseGen.SetCellularJitter(mCellularJitterModifier);
            veinLocationNoiseGen.SetDomainWarpType(mDomainWarpType);
            veinLocationNoiseGen.SetDomainWarpAmp(mDomainWarpAmp);

            veinTypeNoiseGen = new FastNoiseLite();
            veinTypeNoiseGen.SetFrequency(mFrequency*.5f);
            veinTypeNoiseGen.SetNoiseType(mNoiseType);
            veinTypeNoiseGen.SetFractalType(mFractalType);
            veinTypeNoiseGen.SetFractalOctaves(mOctaves);
            veinTypeNoiseGen.SetFractalLacunarity(mLacunarity);
            veinTypeNoiseGen.SetFractalGain(mGain);
            veinTypeNoiseGen.SetFractalWeightedStrength(mWeightedStrength);
            veinTypeNoiseGen.SetFractalPingPongStrength(mPingPongStrength);
            veinTypeNoiseGen.SetCellularDistanceFunction(mCellularDistanceFunction);
            veinTypeNoiseGen.SetCellularReturnType(mCellularReturnType);
            veinTypeNoiseGen.SetCellularJitter(mCellularJitterModifier);
            veinTypeNoiseGen.SetDomainWarpType(mDomainWarpType);
            veinTypeNoiseGen.SetDomainWarpAmp(mDomainWarpAmp);

            baseRockNoiseGen = new FastNoiseLite();
            baseRockNoiseGen.SetFrequency(mFrequency*7f);
            baseRockNoiseGen.SetNoiseType(mNoiseType);
            baseRockNoiseGen.SetFractalType(mFractalType);
            baseRockNoiseGen.SetFractalOctaves(mOctaves);
            baseRockNoiseGen.SetFractalLacunarity(mLacunarity);
            baseRockNoiseGen.SetFractalGain(mGain);
            baseRockNoiseGen.SetFractalWeightedStrength(mWeightedStrength);
            baseRockNoiseGen.SetFractalPingPongStrength(mPingPongStrength);
            baseRockNoiseGen.SetCellularDistanceFunction(mCellularDistanceFunction);
            baseRockNoiseGen.SetCellularReturnType(mCellularReturnType);
            baseRockNoiseGen.SetCellularJitter(mCellularJitterModifier);
            baseRockNoiseGen.SetDomainWarpType(mDomainWarpType);
            baseRockNoiseGen.SetDomainWarpAmp(mDomainWarpAmp);


            Seed = seed;

            if (veinHeight <= 0 || veinHeight >= 1)
                throw new ArgumentException("Invalid Vein Height");
            this.veinHeight = veinHeight;
            this.rockVariants = new List<Resource>(rockVariants);
            this.ores = new List<Ore>(ores);

            for (int i = 0; i < this.ores.Count; i++)
            {
                Ore ore = this.ores[i];
                Sprite newSprite = CombineSprites(rockVariants[2].tile.sprite, ore.resource.tile.sprite);

                // Make a copy of the Resource (assuming Resource is a class; clone if needed)
                var newResource = new Resource(ore.resource);
                Tile newTile = UnityEngine.Object.Instantiate(ore.resource.tile);
                newTile.sprite = newSprite;
                newResource.tile = newTile;

                // Replace the struct in the list with a modified copy
                this.ores[i] = new Ore(newResource, ore.spawnChance);
            }

        }

        public float GetVeinLocationNoise(Point point) => veinLocationNoiseGen.GetNoise(point.X, point.Y);
        public float GetVeinTypeNoise(Point point) => veinTypeNoiseGen.GetNoise(point.X, point.Y);
        public float GetBaseRockNoise(Point point) => baseRockNoiseGen.GetNoise(point.X, point.Y);

        public Resource GetResource(Point point)
        {
            float noiseValue = (GetVeinLocationNoise(point)+1)/2;

            if (noiseValue < veinHeight)
            {
                float rockNoise = (GetBaseRockNoise(point)+1)/2;
                if(rockNoise >= .9f)
                    return rockVariants[1]; //rock 7 .9f-1
                else if(rockNoise >= .2f)
                    return rockVariants[2]; //rock 14 .2f -.9f
                return rockVariants[0]; //rock 0 0-.2f
            }

            float totalSpawnRate = 0;
            
            totalSpawnRate = ores.Sum(x => x.spawnChance);

            float veinNoiseValue = (GetVeinTypeNoise(point) + 1) / 2; // Normalize to 0-1
            float weightedValue = veinNoiseValue * totalSpawnRate/2;
            float cumulative = 0;
            
                for (int i = 0; i < ores.Count; i += 2)
                {
                    cumulative += ores[i].spawnChance;
                    if (weightedValue <= cumulative)
                    {
                        return ores[i+ (int)(veinNoiseValue * 10000) % 2].resource; //return a random varaition of the ore
                    }
                }
            
            return ores[^1].resource; // Default to the last ore in case of rounding issues
        }

        public static Sprite CombineSprites(Sprite background, Sprite overlay)
        {
            const int width = 256;
            const int height = 256;

            var combined = new Texture2D(width, height, TextureFormat.ARGB32, false);

            // Get pixels
            UnityEngine.Color[] basePixels = GetSpritePixels(background);
            UnityEngine.Color[] overlayPixels = overlay.texture.GetPixels();

            int baseWidth = Mathf.FloorToInt(background.textureRect.width);
            int baseHeight = Mathf.FloorToInt(background.textureRect.height);
            int overlayWidth = Mathf.FloorToInt(overlay.textureRect.width);
            int overlayHeight = Mathf.FloorToInt(overlay.textureRect.height);

            // Apply base pixels first
            combined.SetPixels(0, 0, baseWidth, baseHeight, basePixels);

            // Calculate where to start the overlay (centered)
            int offsetX = (width - overlayWidth) / 2;
            int offsetY = (height - overlayHeight) / 2;

            for (int y = 0; y < overlayHeight; y++)
            {
                for (int x = 0; x < overlayWidth; x++)
                {
                    int overlayIndex = y * overlayWidth + x;
                    int combinedX = offsetX + x;
                    int combinedY = offsetY + y;

                    // Skip out-of-bounds just in case
                    if (combinedX < 0 || combinedX >= width || combinedY < 0 || combinedY >= height)
                        continue;

                    UnityEngine.Color fg = overlayPixels[overlayIndex];
                    UnityEngine.Color bg = combined.GetPixel(combinedX, combinedY);

                    UnityEngine.Color blended = fg.a * fg + (1 - fg.a) * bg;
                    combined.SetPixel(combinedX, combinedY, blended);
                }
            }

            combined.Apply();

            return Sprite.Create(combined, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), background.pixelsPerUnit);
        }


        public static UnityEngine.Color[] GetSpritePixels(Sprite sprite)
        {
            var tex = sprite.texture;
            var rect = sprite.textureRect;

            int x = Mathf.FloorToInt(rect.x);
            int y = Mathf.FloorToInt(rect.y);
            int width = Mathf.FloorToInt(rect.width);
            int height = Mathf.FloorToInt(rect.height);

            return tex.GetPixels(x, y, width, height);
        }

    }
}
