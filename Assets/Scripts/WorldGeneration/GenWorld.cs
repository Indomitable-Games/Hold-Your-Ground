using Assets.Scripts;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


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
            UnityEngine.Debug.LogError("Player not found! Make sure the Player GameObject has the correct tag.");
        }
        
        int width = 9 * playerGenDistance; //TODO: Orthographic Size
        width += width % 6;
        chunkSize = width / 3;

        world.ClearAllTiles();
        List<Vector2Int> chunkOffsets = new List<Vector2Int>
        {
            Vector2Int.zero,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.down + Vector2Int.right
        };
        GenChunkHandlerFromCoords(chunkOffsets);
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
/*
    private void GenChunkHandlerFromCoords(List<Vector2Int> chunkCoords)
    {
        StartCoroutine(GenerateChunksHybridCoroutine(chunkCoords));
    }
    private IEnumerator GenerateChunksHybridCoroutine(List<Vector2Int> chunkCoords)
    {
        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

        // Prune old chunks
        while (last.Count > Globals.lastChunks)
        {
            Vector3Int[] toRemove = last.Dequeue();
            world.SetTiles(toRemove, new TileBase[toRemove.Length]);
            yield return null;
        }

        if (chunkCoords.Count == 0)
            yield break;

        Vector3 originPos = transform.position; //Cache before going into background threads

        // Kick off all chunk generation tasks
        List<Task<(Vector3Int[], TileBase[])>> generationTasks = new();
        foreach (Vector2Int chunkCoord in chunkCoords)
        {
            generationTasks.Add(Task.Run(() => GenerateChunkDataSync(chunkCoord, originPos)));
        }

        // Wait until all tasks are complete
        while (!Task.WhenAll(generationTasks).IsCompleted)
            yield return null;

        // Apply tiles on main thread
        foreach (var task in generationTasks)
        {
            (Vector3Int[] positions, TileBase[] tiles) = task.Result;
            world.SetTiles(positions, tiles);
            last.Enqueue(positions);
            yield return null; // small delay to keep things smooth
        }

        sw.Stop();
        UnityEngine.Debug.Log($"Hybrid Chunk Batch Done in {sw.ElapsedMilliseconds} ms");
    }
    private (Vector3Int[], TileBase[]) GenerateChunkDataSync(Vector2Int chunkOffset, Vector3 origin)
    {
        List<Vector3Int> positions = new();
        List<TileBase> tiles = new();

        int baseX = (int)origin.x + chunkOffset.x * chunkSize;
        int baseY = (int)origin.y + chunkOffset.y * chunkSize;

        for (int y = 0; y < chunkSize; y++)
        {
            int worldY = baseY + y;
            for (int x = -chunkSize / 2; x < chunkSize / 2; x++)
            {
                int worldX = baseX + x;
                Vector3Int pos = new(worldX, worldY, 0);
                TileBase tile = Globals.PlanetList[Globals.planetID].GetResource(
                    new System.Drawing.Point(worldX, worldY)
                );

                positions.Add(pos);
                tiles.Add(tile);
            }
        }

        return (positions.ToArray(), tiles.ToArray());
    }
*/
/*
    
        private void GenChunkHandlerFromCoords(List<Vector2Int> chunkCoords)
        {
            StartCoroutine(GenerateChunksCoroutine(chunkCoords));
        }

        private IEnumerator GenerateChunksCoroutine(List<Vector2Int> chunkCoords)
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            // Prune old chunks
            while (last.Count > Globals.lastChunks)
            {
                Vector3Int[] positionsToRemove = last.Dequeue();
                world.SetTiles(positionsToRemove, new TileBase[positionsToRemove.Length]);
                yield return null;
            }

            // Generate each chunk sequentially to avoid performance spikes
            foreach (Vector2Int chunkCoord in chunkCoords)
            {
                yield return StartCoroutine(GenerateSingleChunkCoroutine(chunkCoord));
            }

            sw.Stop();
            UnityEngine.Debug.Log($"Chunk batch complete in {sw.ElapsedMilliseconds} ms");
        }

        // Coroutine to generate a single chunk (calls GetChunkRowAsync per row)
        private IEnumerator GenerateSingleChunkCoroutine(Vector2Int chunkOffset)
        {
            List<Vector3Int> positions = new();
            List<TileBase> tiles = new();

            for (int y = 0; y < chunkSize; y++)
            {
                System.Drawing.Point start = new()
                {
                    X = (int)transform.position.x + chunkOffset.x * chunkSize,
                    Y = (int)transform.position.y + chunkOffset.y * chunkSize + y
                };

                // Get one row (synchronously now)
                (Vector3Int[] rowPositions, TileBase[] rowTiles) = GetChunkRow(start, chunkSize);
                positions.AddRange(rowPositions);
                tiles.AddRange(rowTiles);

                // Spread workload: yield every 2 rows
                if (y % 2 == 0)
                    yield return null;
            }

            world.SetTiles(positions.ToArray(), tiles.ToArray());
            last.Enqueue(positions.ToArray());
        }

        private (Vector3Int[], TileBase[]) GetChunkRow(System.Drawing.Point start, int width)
        {
            Vector3Int[] vector3Ints = new Vector3Int[width];
            TileBase[] tileArr = new TileBase[width];
            for (int x = -width / 2; x < width / 2; x++)
            {
                int pos = x + (width / 2);
                vector3Ints[pos] = new Vector3Int(x + start.X, start.Y, 0);
                tileArr[pos] = Globals.PlanetList[Globals.planetID].GetResource(
                    new System.Drawing.Point(vector3Ints[pos].x, vector3Ints[pos].y)
                );
            }

            return (vector3Ints, tileArr);
        }
    */
    /*

        private void GenChunkHandlerFromCoords(List<Vector2Int> chunkCoords)
        {
            Stopwatch sw = Stopwatch.StartNew();

            while (last.Count > Globals.lastChunks)
            {
                Stopwatch pruneTimer = Stopwatch.StartNew();
                Vector3Int[] positionsToRemove = last.Dequeue();
                world.SetTiles(positionsToRemove, new TileBase[positionsToRemove.Length]);
                pruneTimer.Stop();
                UnityEngine.Debug.Log($"Pruned chunk in {pruneTimer.ElapsedMilliseconds} ms");
            }

            if (chunkCoords.Count == 0) return;

            List<Task<(Vector3Int[], TileBase[])>> chunkTasks = new();
            foreach (Vector2Int coord in chunkCoords)
            {
                Stopwatch singleChunkTimer = Stopwatch.StartNew();
                chunkTasks.Add(GenerateChunkDataAsync(coord));
                singleChunkTimer.Stop();
                UnityEngine.Debug.Log($"Scheduled chunk {coord} in {singleChunkTimer.ElapsedMilliseconds} ms");
            }

            Task.WaitAll(chunkTasks.ToArray());

            Stopwatch tileSetTimer = Stopwatch.StartNew();
            foreach (var task in chunkTasks)
            {
                world.SetTiles(task.Result.Item1, task.Result.Item2);
                last.Enqueue(task.Result.Item1);
            }
            tileSetTimer.Stop();

            sw.Stop();
            UnityEngine.Debug.Log($"[GenChunkHandlerFromCoords] Total time: {sw.ElapsedMilliseconds} ms");
            UnityEngine.Debug.Log($"[GenChunkHandlerFromCoords] Tilemap.SetTiles time: {tileSetTimer.ElapsedMilliseconds} ms");
        }

        private async Task<(Vector3Int[], TileBase[])> GenerateChunkDataAsync(Vector2Int chunkOffset)
        {
            Stopwatch sw = Stopwatch.StartNew();

            List<Vector3Int> vector3Ints = new List<Vector3Int>();
            List<TileBase> tileArr = new List<TileBase>();
            List<Task<(Vector3Int[], TileBase[])>> rowTasks = new();

            for (int y = 0; y < chunkSize; y++)
            {
                System.Drawing.Point start = new System.Drawing.Point(
                    (int)transform.position.x + chunkOffset.x * chunkSize,
                    (int)transform.position.y + chunkOffset.y * chunkSize + y
                );
                rowTasks.Add(GetChunkRowAsync(start, chunkSize));
            }

            Task.WaitAll(rowTasks.ToArray());

            foreach (var task in rowTasks)
            {
                vector3Ints.AddRange(task.Result.Item1);
                tileArr.AddRange(task.Result.Item2);
            }

            sw.Stop();
            UnityEngine.Debug.Log($"Chunk {chunkOffset} generated in {sw.ElapsedMilliseconds} ms");

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
    */
}
