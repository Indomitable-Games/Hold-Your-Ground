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

        public Resource resource;  // Could use resource ID or name
        public float spawnChance;
        public Ore(Resource resource, float spawChance)
        {
            this.resource = resource;
            this.spawnChance = spawChance;
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
        public List<int> BattleDepths { get; }
        public Planet(PlanetConfigDataModel planet)
        {
            veinLocationNoiseGen = new FastNoiseLite();
            veinLocationNoiseGen.SetFrequency(planet.frequency);
            veinLocationNoiseGen.SetNoiseType(planet.noiseType);
            veinLocationNoiseGen.SetFractalType(planet.fractalType);
            veinLocationNoiseGen.SetFractalOctaves(planet.octaves);
            veinLocationNoiseGen.SetFractalLacunarity(planet.lacunarity);
            veinLocationNoiseGen.SetFractalGain(planet.gain);
            veinLocationNoiseGen.SetFractalWeightedStrength(planet.weightedStrength);
            veinLocationNoiseGen.SetFractalPingPongStrength(planet.pingPongStrength);
            veinLocationNoiseGen.SetCellularDistanceFunction(planet.cellularDistanceFunction);
            veinLocationNoiseGen.SetCellularReturnType(planet.cellularReturnType);
            veinLocationNoiseGen.SetCellularJitter(planet.cellularJitterModifier);
            veinLocationNoiseGen.SetDomainWarpType(planet.domainWarpType);
            veinLocationNoiseGen.SetDomainWarpAmp(planet.domainWarpAmp);

            veinTypeNoiseGen = new FastNoiseLite();
            veinTypeNoiseGen.SetFrequency(planet.frequency * .5f);
            veinTypeNoiseGen.SetNoiseType(planet.noiseType);
            veinTypeNoiseGen.SetFractalType(planet.fractalType);
            veinTypeNoiseGen.SetFractalOctaves(planet.octaves);
            veinTypeNoiseGen.SetFractalLacunarity(planet.lacunarity);
            veinTypeNoiseGen.SetFractalGain(planet.gain);
            veinTypeNoiseGen.SetFractalWeightedStrength(planet.weightedStrength);
            veinTypeNoiseGen.SetFractalPingPongStrength(planet.pingPongStrength);
            veinTypeNoiseGen.SetCellularDistanceFunction(planet.cellularDistanceFunction);
            veinTypeNoiseGen.SetCellularReturnType(planet.cellularReturnType);
            veinTypeNoiseGen.SetCellularJitter(planet.cellularJitterModifier);
            veinTypeNoiseGen.SetDomainWarpType(planet.domainWarpType);
            veinTypeNoiseGen.SetDomainWarpAmp(planet.domainWarpAmp);
            

            baseRockNoiseGen = new FastNoiseLite();
            baseRockNoiseGen.SetFrequency(planet.frequency*7f);
            baseRockNoiseGen.SetNoiseType(planet.noiseType);
            baseRockNoiseGen.SetFractalType(planet.fractalType);
            baseRockNoiseGen.SetFractalOctaves(planet.octaves);
            baseRockNoiseGen.SetFractalLacunarity(planet.lacunarity);
            baseRockNoiseGen.SetFractalGain(planet.gain);
            baseRockNoiseGen.SetFractalWeightedStrength(planet.weightedStrength);
            baseRockNoiseGen.SetFractalPingPongStrength(planet.pingPongStrength);
            baseRockNoiseGen.SetCellularDistanceFunction(planet.cellularDistanceFunction);
            baseRockNoiseGen.SetCellularReturnType(planet.cellularReturnType);
            baseRockNoiseGen.SetCellularJitter(planet.cellularJitterModifier);
            baseRockNoiseGen.SetDomainWarpType(planet.domainWarpType);
            baseRockNoiseGen.SetDomainWarpAmp(planet.domainWarpAmp);


            Seed = planet.seed;

            veinHeight = planet.veinHeight;

            if (veinHeight <= 0 || veinHeight >= 1)
                throw new ArgumentException("Invalid Vein Height");

            rockVariants = new List<Resource>();
            foreach(string rocks in planet.rockVariantNames)
                rockVariants.Add(Globals.ResourceDictionary[rocks]);
            ores = new List<Ore>(); //TODO ADD ORES

            foreach (OreConfig ore in planet.ores)
            {
                ores.Add(new Ore(Globals.ResourceDictionary[ore.resourceName], ore.spawnChance));
            }


            for (int i = 0; i < this.ores.Count; i++)
            {
                Ore ore = ores[i];
                Sprite newSprite = CombineSprites(rockVariants[2].tile.sprite, ore.resource.tile.sprite);

                // Make a copy of the Resource (assuming Resource is a class; clone if needed)
                var newResource = new Resource(ore.resource);
                Tile newTile = UnityEngine.Object.Instantiate(ore.resource.tile);
                newTile.sprite = newSprite;
                newResource.tile = newTile;

                // Replace the struct in the list with a modified copy
                this.ores[i] = new Ore(newResource, ore.spawnChance);
            }
            BattleDepths = planet.battleDepths;
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
