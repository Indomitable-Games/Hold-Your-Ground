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
        public Resource BaseRock {get;}
        public List<Ore> Resources { get; }
        public List<BattleConfigDataModel> battles { get; }
        public int battleIndex { get; }
        public Planet(PlanetConfigDataModel planet)
        {
            #region NoiseGenerators init
            veinLocationNoiseGen = new FastNoiseLite();
            veinLocationNoiseGen.SetFrequency(planet.Frequency);
            veinLocationNoiseGen.SetNoiseType(planet.NoiseType);
            veinLocationNoiseGen.SetFractalType(planet.FractalType);
            veinLocationNoiseGen.SetFractalOctaves(planet.Octaves);
            veinLocationNoiseGen.SetFractalLacunarity(planet.Lacunarity);
            veinLocationNoiseGen.SetFractalGain(planet.Gain);
            veinLocationNoiseGen.SetFractalWeightedStrength(planet.WeightedStrength);
            veinLocationNoiseGen.SetFractalPingPongStrength(planet.PingPongStrength);
            veinLocationNoiseGen.SetCellularDistanceFunction(planet.CellularDistanceFunction);
            veinLocationNoiseGen.SetCellularReturnType(planet.CellularReturnType);
            veinLocationNoiseGen.SetCellularJitter(planet.CellularJitterModifier);
            veinLocationNoiseGen.SetDomainWarpType(planet.DomainWarpType);
            veinLocationNoiseGen.SetDomainWarpAmp(planet.DomainWarpAmp);

            veinTypeNoiseGen = new FastNoiseLite();
            veinTypeNoiseGen.SetFrequency(planet.Frequency * .5f);
            veinTypeNoiseGen.SetNoiseType(planet.NoiseType);
            veinTypeNoiseGen.SetFractalType(planet.FractalType);
            veinTypeNoiseGen.SetFractalOctaves(planet.Octaves);
            veinTypeNoiseGen.SetFractalLacunarity(planet.Lacunarity);
            veinTypeNoiseGen.SetFractalGain(planet.Gain);
            veinTypeNoiseGen.SetFractalWeightedStrength(planet.WeightedStrength);
            veinTypeNoiseGen.SetFractalPingPongStrength(planet.PingPongStrength);
            veinTypeNoiseGen.SetCellularDistanceFunction(planet.CellularDistanceFunction);
            veinTypeNoiseGen.SetCellularReturnType(planet.CellularReturnType);
            veinTypeNoiseGen.SetCellularJitter(planet.CellularJitterModifier);
            veinTypeNoiseGen.SetDomainWarpType(planet.DomainWarpType);
            veinTypeNoiseGen.SetDomainWarpAmp(planet.DomainWarpAmp);


            baseRockNoiseGen = new FastNoiseLite();
            baseRockNoiseGen.SetFrequency(planet.Frequency*7f);
            baseRockNoiseGen.SetNoiseType(planet.NoiseType);
            baseRockNoiseGen.SetFractalType(planet.FractalType);
            baseRockNoiseGen.SetFractalOctaves(planet.Octaves);
            baseRockNoiseGen.SetFractalLacunarity(planet.Lacunarity);
            baseRockNoiseGen.SetFractalGain(planet.Gain);
            baseRockNoiseGen.SetFractalWeightedStrength(planet.WeightedStrength);
            baseRockNoiseGen.SetFractalPingPongStrength(planet.PingPongStrength);
            baseRockNoiseGen.SetCellularDistanceFunction(planet.CellularDistanceFunction);
            baseRockNoiseGen.SetCellularReturnType(planet.CellularReturnType);
            baseRockNoiseGen.SetCellularJitter(planet.CellularJitterModifier);
            baseRockNoiseGen.SetDomainWarpType(planet.DomainWarpType);
            baseRockNoiseGen.SetDomainWarpAmp(planet.DomainWarpAmp);
            #endregion

            Seed = planet.Seed;

            veinHeight = planet.VeinHeight;

            if (veinHeight <= 0 || veinHeight >= 1)
                throw new ArgumentException("Invalid Vein Height");

            #region tileInfo
            BaseRock = Globals.ResourceDictionary[planet.Tiles[0].ResourceName];
            Resources = new List<Ore>(); //TODO ADD ORES

            foreach (OreConfig ore in planet.Tiles.Skip(1))
                Resources.Add(new Ore(Globals.ResourceDictionary[ore.ResourceName],ore.SpawnChance));
            
            for (int i = 0; i < this.Resources.Count; i++)
            {
                Ore ore = Resources[i];
                var newResource = new Resource(ore.resource);

                for (int j = 0; j < Resources[i].resource.Tiles.Count; j++)
                {
                    Sprite newSprite = CombineSprites(BaseRock.Tiles[0].sprite, ore.resource.Tiles[j].sprite);

                    // Make a copy of the Resource (assuming Resource is a class; clone if needed)
                    Tile newTile = UnityEngine.Object.Instantiate(ore.resource.Tiles[j]);
                    newTile.name = ore.resource.Tiles[j].name;
                    newTile.sprite = newSprite;
                    newResource.Tiles[j] = newTile;

                }
                // Replace the struct in the list with a modified copy
                Resources[i] = new Ore(newResource, ore.spawnChance);
            }
            #endregion

            battles = planet.BattleConfigInfo;
        }

        public float GetVeinLocationNoise(Point point) => veinLocationNoiseGen.GetNoise(point.X, point.Y);
        public float GetVeinTypeNoise(Point point) => veinTypeNoiseGen.GetNoise(point.X, point.Y);
        public float GetBaseRockNoise(Point point) => baseRockNoiseGen.GetNoise(point.X, point.Y);
        public int GetBattleDepth() => battles[battleIndex].Depth;
        public Tile GetResource(Point point)
        {
            float noiseValue = (GetVeinLocationNoise(point)+1)/2;

            //under vein hight, its not a resource, decide which base rock it is
            if (noiseValue < veinHeight)
            {
                float rockNoise = (GetBaseRockNoise(point) + 1) / 2;
                //1-(1-mainchanc)/2 = .9 if mainchance = .8
                if (rockNoise >= 1-((1-BaseRock.mainSpawnChance)/2))
                    return BaseRock.Tiles[1]; //rock 0 .9f-1
                else if(rockNoise < (1-BaseRock.mainSpawnChance)/2)
                    return BaseRock.Tiles[2]; //rock 7 0-.1
                return BaseRock.Tiles[0]; //rock 0 0-.2f
            }

            float totalSpawnRate = 0;
            
            totalSpawnRate = Resources.Sum(x => x.spawnChance);

            float veinNoiseValue = (GetVeinTypeNoise(point) + 1) / 2; // Normalize to 0-1
            float weightedValue = veinNoiseValue * totalSpawnRate;
            float cumulative = 0;
            
                for (int i = 0; i < Resources.Count; i++)
                {
                    cumulative += Resources[i].spawnChance;
                    if (weightedValue <= cumulative)
                    {
                        return Resources[i].resource.Tiles[(int)(veinNoiseValue * 10000) % 2]; //return a random varaition of the ore
                    }
                }
            
            return Resources[^1].resource.Tiles[0]; // Default to the last ore in case of rounding issues
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
