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


    public Tilemap world;

    private GameObject player;
    private int playerGenDistance = 20; //TODO Make orthographic view
    private int chunkSize;


    public Queue<Vector3Int[]> last = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the Player GameObject has the correct tag.");
        }
        
        int width = 9 * playerGenDistance; //TODO: Orthographic Size
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

    private HashSet<Vector2Int> generatedChunks = new();
    private Vector2Int lastPlayerChunk = new(-9999, -9999);


    void Update()
    {
        Vector3 playerPos = player.transform.position;

        int playerChunkX = Mathf.FloorToInt(playerPos.x / chunkSize);
        int playerChunkY = Mathf.FloorToInt(playerPos.y / chunkSize);
        Vector2Int playerChunk = new(playerChunkX, playerChunkY);

        if (playerChunk != lastPlayerChunk)
        {
            lastPlayerChunk = playerChunk;
            GenerateChunksAroundPlayer(playerChunk);
        }

        if (playerPos.y < Globals.PlanetList[Globals.planetID].GetBattleDepth() - 10)
            LoadBattle();
    }
    private void GenerateChunksAroundPlayer(Vector2Int centerChunk)
    {
        List<Vector2Int> chunksToGenerate = new();

        int radius = 1; // 1 = generate a 3x3 grid around the player
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= 0; dy++) // only generate downward if needed
            {
                Vector2Int chunkCoord = new(centerChunk.x + dx, centerChunk.y + dy);
                if (!generatedChunks.Contains(chunkCoord))
                {
                    generatedChunks.Add(chunkCoord);
                    chunksToGenerate.Add(chunkCoord);
                }
            }
        }

        GenChunkHandlerFromCoords(chunksToGenerate);
    }

    private void LoadBattle()
    {
        Destroy(player);
        SceneManager.LoadScene("Battle");
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


    private void GenChunkHandlerFromCoords(List<Vector2Int> chunkCoords)
    {
        if (last.Count > Globals.lastChunks)
        {
            Vector3Int[] positionsToRemove = last.Dequeue();
            world.SetTiles(positionsToRemove, new TileBase[positionsToRemove.Length]);
        }

        if (chunkCoords.Count == 0) return;

        List<Task<(Vector3Int[], TileBase[])>> chunkTasks = new();
        foreach (Vector2Int coord in chunkCoords)
            chunkTasks.Add(GenerateChunkDataAsync(coord));

        Task.WaitAll(chunkTasks.ToArray());

        foreach (var task in chunkTasks)
        {
            world.SetTiles(task.Result.Item1, task.Result.Item2);
            last.Enqueue(task.Result.Item1);
        }
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
