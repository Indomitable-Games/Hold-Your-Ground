using Assets.Scripts;

using System;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

using static FastNoiseLite;

public class GenWorld : MonoBehaviour
{
    #region DataTypes
    [Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
        public TileBase tile;
    }

    public struct MapData
    {
        public readonly float[,] heightMap;
        public readonly Color[] colourMap;
        public readonly TileBase[] tileArr;

        public MapData(float[,] heightMap, Color[] colourMap, TileBase[] tileArr)
        {
            this.heightMap = heightMap;
            this.colourMap = colourMap;
            this.tileArr = tileArr;
        }
    }
    public enum DrawMode { NoiseMap, ColorMap, Tilemap };
    #endregion

    public DrawMode drawMode;

    #region WorldGen Settings
    public int xOffset = 0;
    public int yOffset = 0;
    public int mapChunkSize = 500;
    public int mSeed = 1337;
    public float mFrequency = 0.01f;
    public NoiseType mNoiseType = NoiseType.OpenSimplex2;
    public FractalType mFractalType = FractalType.None;
    public int mOctaves = 3;
    public float mLacunarity = 2.0f;
    public float mGain = 0.5f;
    public float mWeightedStrength = 0.0f;
    public float mPingPongStrength = 2.0f;
    public CellularDistanceFunction mCellularDistanceFunction = CellularDistanceFunction.EuclideanSq;
    public CellularReturnType mCellularReturnType = CellularReturnType.Distance;
    public float mCellularJitterModifier = 1.0f;
    public DomainWarpType mDomainWarpType = DomainWarpType.OpenSimplex2;
    public float mDomainWarpAmp = 1.0f;
    #endregion

    public TerrainType[] regions;
    public Tilemap world;

    private GameObject player;
    public int playerGenDistance = 50;

    public bool autoUpdate;
    public bool ChunkGen;

    public Queue<Vector3Int[]> last = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the Player GameObject has the correct tag.");
        }
        world.ClearAllTiles();
        GenChunk(Vector3.zero);
    }


    public Renderer textureRender;

    public void DrawMapInEditor()
    {
        int width = (int)(2 * (mapChunkSize + playerGenDistance) * (Mathf.Tan(Assets.Scripts.Globals.playerTurnRadius * Mathf.Deg2Rad))) + 4 * playerGenDistance;
        if (width % 2 == 1)
            width++;
        MapData mapData = GenerateMapData(xOffset, yOffset, width, mapChunkSize);

        if (drawMode == DrawMode.NoiseMap)
        {
            if (textureRender == null)
                return;
            Texture2D texture = TextureGenerator.TextureFromHeightMap(mapData.heightMap);
            textureRender.sharedMaterial.mainTexture = texture;
            textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            if (textureRender == null)
                return;
            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colourMap, mapChunkSize, mapChunkSize);
            textureRender.sharedMaterial.mainTexture = texture;
            textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }
        else if (drawMode == DrawMode.Tilemap)
        {
            Vector3Int[] vector3Ints = new Vector3Int[width * mapChunkSize];
            for (int y = 0; y < mapChunkSize; y++)
                for (int x = -width / 2; x < width / 2; x++)
                    vector3Ints[y*width+x + (width / 2)] = new Vector3Int(x, -y, 0);

            world.ClearAllTiles();
            world.SetTiles(vector3Ints, mapData.tileArr);
            last.Enqueue(vector3Ints);
        }
    }





    MapData GenerateMapData(int xOffset, int yOffset, int width, int height)
    {
        float[,] noiseMap;

        noiseMap = GenerateNoiseMapFromFast(xOffset, yOffset, width, height, mSeed, mFrequency, mNoiseType, mFractalType, mOctaves, mLacunarity, mGain, mWeightedStrength, mPingPongStrength, mCellularDistanceFunction, mCellularReturnType, mCellularJitterModifier, mDomainWarpType, mDomainWarpAmp);

        Color[] colorMap = new Color[width * height];
        TileBase[] tileArr = new TileBase[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0;x < width; x++)
            {

                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        if (yOffset + y > Math.Abs(Globals.battleDepth))
                        {
                            if (yOffset + y > Math.Abs(Globals.battleDepth) + 5)
                            {
                                colorMap[y * width + x] = Color.clear;
                                tileArr[y * width + x] = null;
                            }
                            else
                            {
                                System.Random rand= new System.Random();
                                int value = rand.Next(5);
                                colorMap[y * width + x] = (value < (Math.Abs(Globals.battleDepth) + 5)-(yOffset + y) ? regions[i].color : regions[^1].color);
                                tileArr[y * width + x] = (value < (Math.Abs(Globals.battleDepth) + 5) - (yOffset + y) ? regions[i].tile : regions[^1].tile);
                            }
                        }
                        else
                        {
                            colorMap[y * width + x] = regions[i].color;
                            tileArr[y * width + x] = regions[i].tile;
                        }
                        

                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap, tileArr);
    }


    public static float[,] GenerateNoiseMapFromFast(int xOffset, int yOffset, int width, int height, int seed, float frequency, FastNoiseLite.NoiseType noiseType, FastNoiseLite.FractalType fractalType, int octaves, float lacunarity, float gain, float weightedStrength, float pingPongStrength, FastNoiseLite.CellularDistanceFunction cellularDistanceFunction, FastNoiseLite.CellularReturnType cellularReturnType, float cellularJitter, FastNoiseLite.DomainWarpType domainWarpType, float domainWarpAmp)
    {
        float[,] noiseMapReturn = new float[width, height];
        FastNoiseLite noiseMap = new FastNoiseLite(seed);
        noiseMap.SetFrequency(frequency);
        noiseMap.SetNoiseType(noiseType);
        noiseMap.SetFractalType(fractalType);
        noiseMap.SetFractalOctaves(octaves);
        noiseMap.SetFractalLacunarity(lacunarity);
        noiseMap.SetFractalGain(gain);
        noiseMap.SetFractalWeightedStrength(weightedStrength);
        noiseMap.SetFractalPingPongStrength(pingPongStrength);
        noiseMap.SetCellularDistanceFunction(cellularDistanceFunction);
        noiseMap.SetCellularReturnType(cellularReturnType);
        noiseMap.SetCellularJitter(cellularJitter);
        noiseMap.SetDomainWarpType(domainWarpType);
        noiseMap.SetDomainWarpAmp(domainWarpAmp);

        //float largest = float.MinValue;
        //float smallest = float.MaxValue;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                noiseMapReturn[x, y] = noiseMap.GetNoise(xOffset + x, yOffset + y);
                /*if(noiseMapReturn[x,y] > largest)
                    largest = noiseMapReturn[x,y];
                else if (noiseMapReturn[x,y] < smallest)
                    smallest = noiseMapReturn[x,y];*/
            }
        //Debug.Log($"Largest:{largest}, smallest:{smallest}");
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                noiseMapReturn[x, y] = (noiseMapReturn[x, y] + 1) / 2;
            }
        return noiseMapReturn;
    }

    void Update()
    {
        if (!ChunkGen)
        { return; }
        Vector3 playerPos = player.transform.position;
        if (playerPos.y < this.gameObject.transform.position.y - mapChunkSize + playerGenDistance)
            GenChunk(new Vector3(player.transform.position.x, this.transform.position.y - mapChunkSize));
        if (playerPos.y < Globals.battleDepth-10)
            LoadBattle();


    }

    private void LoadBattle()
    {
        Destroy(player);
        SceneManager.LoadScene("Battle");

    }

    public void GenChunk(Vector3 origin)
    {
        if (last.Count > Globals.LastChunks)
            foreach (Vector3Int p in last.Dequeue())
                world.SetTile(p, null);
        if (origin.y < Globals.battleDepth-10)
            return;
        int width = (int)(2 * (mapChunkSize+playerGenDistance) * (Mathf.Tan(Assets.Scripts.Globals.playerTurnRadius * Mathf.Deg2Rad))) + 4*playerGenDistance;
        if (width % 2 == 1)
            width++;
        
        this.transform.position = origin;
        MapData mapData = GenerateMapData((int)this.transform.position.x, -(int)this.transform.position.y, width, mapChunkSize);
        


        Vector3Int[] vector3Ints = new Vector3Int[width * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
            for (int x = -width / 2; x < width / 2; x++)
                vector3Ints[y * width + x + (width / 2)] = new Vector3Int(x, -y, 0) + Vector3Int.RoundToInt(this.transform.position);

        world.SetTiles(vector3Ints, mapData.tileArr);
        last.Enqueue(vector3Ints);
    }
}
