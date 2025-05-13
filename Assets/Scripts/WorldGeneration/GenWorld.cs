using Assets.Scripts;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

using static FastNoiseLite;
using static UnityEngine.UI.Image;

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
    private int playerGenDistance = 20; //TODO Make orthographic view
    private int chunkSize;

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
        
        int width = 15 * playerGenDistance; //TODO: Orthographic Size
        width += width % 6;
        chunkSize = width / 3;

        world.ClearAllTiles();
        Vector2Int[] chunkOffsets = new Vector2Int[]
        {
            Vector2Int.zero,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.down + Vector2Int.right
        };
        GenChunkHandler(player.transform.position, chunkOffsets);
    }


    public Renderer textureRender;

    public void DrawMapInEditor()
    {
        int width = (int)(2 * (mapChunkSize + playerGenDistance) * (Mathf.Tan(Assets.Scripts.Globals.Player.TurnRadius * Mathf.Deg2Rad))) + 4 * playerGenDistance;
        width += width % 2;
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
                        if (yOffset + y > Math.Abs(Globals.PlanetList[Globals.planetID].GetBattleDepth()))
                        {
                            if (yOffset + y > Math.Abs(Globals.PlanetList[Globals.planetID].GetBattleDepth()) + 5)
                            {
                                colorMap[y * width + x] = Color.clear;
                                tileArr[y * width + x] = null;
                            }
                            else
                            {
                                System.Random rand= new System.Random();
                                int value = rand.Next(5);
                                colorMap[y * width + x] = (value < (Math.Abs(Globals.PlanetList[Globals.planetID].GetBattleDepth()) + 5)-(yOffset + y) ? regions[i].color : regions[^1].color);
                                tileArr[y * width + x] = (value < (Math.Abs(Globals.PlanetList[Globals.planetID].GetBattleDepth()) + 5) - (yOffset + y) ? regions[i].tile : regions[^1].tile);
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
        if (!ChunkGen) return;

        Vector3 playerPos = player.transform.position;
        Vector3 origin = transform.position;

        int playerChunkX = Mathf.FloorToInt(playerPos.x / chunkSize);
        int playerChunkY = Mathf.FloorToInt(playerPos.y / chunkSize);
        int currentChunkX = Mathf.FloorToInt(origin.x / chunkSize);
        int currentChunkY = Mathf.FloorToInt(origin.y / chunkSize);

        // Only update when player enters a new chunk
        if (playerChunkX != currentChunkX || playerChunkY != currentChunkY)
        {
            Vector3 newOrigin = new Vector3(playerChunkX * chunkSize, playerChunkY * chunkSize, 0);
            transform.position = newOrigin;
            Debug.Log($"Moved origin to {newOrigin}");

            // Distances to nearby chunks (in chunk units)
            int distDown = Mathf.Abs(playerChunkY - (currentChunkY - 1));
            int distLeft = Mathf.Abs(playerChunkX - (currentChunkX - 1));
            int distRight = Mathf.Abs(playerChunkX - (currentChunkX + 1));

            GenChunkHandler(newOrigin, GetChunkOffsets(distDown, distLeft, distRight));
        }

        if (playerPos.y < Globals.PlanetList[Globals.planetID].GetBattleDepth() - 10)
            LoadBattle();
    }

    private void LoadBattle()
    {
        Destroy(player);
        SceneManager.LoadScene("Battle");

    }

    public void GenChunk(Vector3 origin)
    {
        if (last.Count > Globals.lastChunks)
        {
            Vector3Int[] positionsToRemove = last.Dequeue();
            world.SetTiles(positionsToRemove, new TileBase[positionsToRemove.Length]);
        }

        if (origin.y < Globals.PlanetList[Globals.planetID].GetBattleDepth() -10)
            return;

        int width = (int)(2 * (mapChunkSize+playerGenDistance) * (Mathf.Tan(Globals.Player.TurnRadius * Mathf.Deg2Rad))) + 4*playerGenDistance;
        if (width % 2 == 1)
            width++;
        
        this.transform.position = origin;
        MapData mapData = GenerateMapData((int)this.transform.position.x, -(int)this.transform.position.y, width, mapChunkSize);
        


        Vector3Int[] vector3Ints = new Vector3Int[width * mapChunkSize];
        TileBase[] tileArr = new TileBase[width * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
            for (int x = -width / 2; x < width / 2; x++)
            {
                int pos = y * width + x + (width / 2);
                vector3Ints[pos] = new Vector3Int(x, -y, 0) + Vector3Int.RoundToInt(this.transform.position);
                tileArr[pos] = Globals.PlanetList[Globals.planetID].GetResource(new System.Drawing.Point(vector3Ints[pos].x,vector3Ints[pos].y));
            }

        world.SetTiles(vector3Ints, tileArr);
        last.Enqueue(vector3Ints);
    }

    public void GenChunksAroundPlayer(Vector3 playerPosition)
    {
        // Calculate player's chunk position (center chunk)
        int playerChunkX = Mathf.FloorToInt(playerPosition.x / chunkSize) - chunkSize / 2;
        int playerChunkY = Mathf.FloorToInt(playerPosition.y / chunkSize) + chunkSize;

        // Debug player position and chunk calculation
        Debug.Log($"Player position: {playerPosition}, Chunk coords: ({playerChunkX}, {playerChunkY})");

        // Define the 5 chunks we want to generate (half-circle below and to the sides)
        Vector2Int[] relativeChunkPositions = new Vector2Int[]
        {
        new Vector2Int(-1, 0),   // West
        new Vector2Int(-1, -1),  // Southwest
        new Vector2Int(0, -1),   // South
        new Vector2Int(1, -1),   // Southeast
        new Vector2Int(1, 0)     // East
        };

        // Generate each chunk
        foreach (Vector2Int relPos in relativeChunkPositions)
        {
            // Calculate absolute chunk position
            int chunkX = playerChunkX + relPos.x;
            int chunkY = playerChunkY + relPos.y;

            // Calculate world position for chunk origin
            Vector3 chunkOrigin = new Vector3(
                chunkX * chunkSize,
                chunkY * chunkSize,
                0
            );

            // Debug the chunk position we're checking
            Debug.Log($"Checking chunk at ({chunkX}, {chunkY}), world pos: {chunkOrigin}");

            // Check if the chunk already exists by looking at its center position
            Vector3Int tilePosToCheck = new Vector3Int(
                (int)chunkOrigin.x + chunkSize / 2,
                (int)chunkOrigin.y - chunkSize / 2,
                0
            );

            // If the tile at this position is null, we need to generate the chunk
            if (world.GetTile(tilePosToCheck) == null)
            {
                Debug.Log($"Generating chunk at ({chunkX}, {chunkY})");
                GenSingleChunk(chunkOrigin);
            }
            else
            {
                Debug.Log($"Chunk at ({chunkX}, {chunkY}) already exists");
            }
        }
    }

    // Generate a single square chunk at the specified origin TODO: Make Async also make assigning resource to tile Async
    public void GenSingleChunk(Vector3 origin)
    {
        // Memory management - remove old chunks if needed
        if (last.Count > Globals.lastChunks)
        {
            Vector3Int[] positionsToRemove = last.Dequeue();
            world.SetTiles(positionsToRemove, new TileBase[positionsToRemove.Length]);
        }

        // Skip if below the battle depth
        if (origin.y < Globals.PlanetList[Globals.planetID].GetBattleDepth() - 10)
            return;

        // Create tile arrays for this chunk
        Vector3Int[] vector3Ints = new Vector3Int[chunkSize * chunkSize];
        TileBase[] tileArr = new TileBase[chunkSize * chunkSize];

        // Fill the tile arrays
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                int index = y * chunkSize + x;

                // Calculate the world position for this tile
                vector3Ints[index] = new Vector3Int(
                    (int)origin.x + x,
                    (int)origin.y - y,
                    0
                );

                // Get resource tile
                tileArr[index] = Globals.PlanetList[Globals.planetID].GetResource(
                    new System.Drawing.Point(vector3Ints[index].x, vector3Ints[index].y));
            }
        }

        // Set tiles in the world
        world.SetTiles(vector3Ints, tileArr);

        // Add to tracking for memory management
        last.Enqueue(vector3Ints);

        // Debug confirmation
        Debug.Log($"Generated chunk at {origin} with size {chunkSize}x{chunkSize}");
    }

    private Vector2Int[] GetChunkOffsets(int distDown, int distLeft, int distRight)
    {
        List<Vector2Int> offsets = new List<Vector2Int>();
        int smallDist = Math.Min(Math.Min(distDown, distLeft), distRight);
        offsets.Add(Vector2Int.down);
        if (smallDist == distDown)
        {
            offsets.Add(Vector2Int.down + Vector2Int.right);
            offsets.Add(Vector2Int.down + Vector2Int.left);
        }
        else if (smallDist == distLeft)
        {
            offsets.Add(Vector2Int.left);
            offsets.Add(Vector2Int.down + Vector2Int.left);
        }
        else
        {
            offsets.Add(Vector2Int.right);
            offsets.Add(Vector2Int.down + Vector2Int.right);
        }
        Debug.Log($"inputs are : {distDown}, {distLeft}, {distRight}, {playerGenDistance}, {smallDist}");
        foreach (Vector2Int offset in offsets)
        {
            Debug.Log($"Gen Chunk at {offset.ToString()}");
        }
        return offsets.ToArray();
    }

    private void GenChunkHandler(Vector3 origin, Vector2Int[] chunkOffsets)
    {
        if (last.Count > Globals.lastChunks)
        {
            Vector3Int[] positionsToRemove = last.Dequeue();
            world.SetTiles(positionsToRemove, new TileBase[positionsToRemove.Length]);
        }

        if (origin.y < Globals.PlanetList[Globals.planetID].GetBattleDepth() - 10)
            return;

        this.transform.position = origin;

        List<Task<(Vector3Int[], TileBase[])>> chunkTasks = new List<Task<(Vector3Int[], TileBase[])>>();
        foreach (Vector2Int chunk in chunkOffsets)
            chunkTasks.Add(GenerateChunkDataAsync(chunk));
        
        Task.WaitAll(chunkTasks.ToArray());
        foreach(Task<(Vector3Int[], TileBase[])> task in chunkTasks)
        {
            world.SetTiles(task.Result.Item1, task.Result.Item2);
            last.Enqueue(task.Result.Item1);
        }
    }

    private async Task<(Vector3Int[], TileBase[])> GenerateChunkDataAsync(Vector2Int chunkOffset)
    {
        List<Vector3Int> vector3Ints = new List<Vector3Int>();
        List<TileBase> tileArr = new List<TileBase>();
        List<Task<(Vector3Int[], TileBase[])>> rowTasks = new List<Task<(Vector3Int[], TileBase[])>>();

        for (int y = 0; y < chunkSize; y++)
        {
            System.Drawing.Point start = new System.Drawing.Point();
            start.X = (int)transform.position.x + chunkOffset.x * chunkSize;
            start.Y = (int)transform.position.y + chunkOffset.y * chunkSize + y;
            rowTasks.Add(GetChunkRowAsync(start, chunkSize));
        }

        Task.WaitAll(rowTasks.ToArray());
        foreach (Task<(Vector3Int[], TileBase[])> task in rowTasks)
        {
            vector3Ints.AddRange(task.Result.Item1);
            tileArr.AddRange(task.Result.Item2);
        }
        return (vector3Ints.ToArray(), tileArr.ToArray());
    }

    private async Task<(Vector3Int[], TileBase[])> GetChunkRowAsync(System.Drawing.Point start, int width)
    {
        Vector3Int[] vector3Ints = new Vector3Int[width];
        TileBase[] tileArr = new TileBase[width];
        for (int x = -width / 2; x < width / 2; x++)
        {
            int pos = x + (width / 2);
            vector3Ints[pos] = new Vector3Int(x + start.X, start.Y, 0);
            tileArr[pos] = Globals.PlanetList[Globals.planetID].GetResource(new System.Drawing.Point(vector3Ints[pos].x, vector3Ints[pos].y));
        }

        return (vector3Ints, tileArr);
    }

}
