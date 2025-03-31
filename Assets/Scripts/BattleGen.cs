using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class BattleGen : MonoBehaviour
{
    public GameObject enemy;
    private GameObject player;
    public Tilemap world;

    public int spawnCount = 10; // Number of enemies to spawn
    public float spawnIntervalMin = 1f; // Minimum spawn interval
    public float spawnIntervalMax = 3f; // Maximum spawn interval
    public int minTileDistance = 5; // Minimum tiles away from the player
    public int maxTileDistance = 10; // Maximum tiles away from the player

    private static System.Random rand;

    void Start()
    {
        rand = new System.Random();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the Player GameObject has the correct tag.");
        }
        else
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (player == null) return;

        int tileOffset = rand.Next(minTileDistance, maxTileDistance + 1);
        int direction = (rand.Next(0, 2) == 0) ? -1 : 1; // Randomly choose left (-1) or right (1)

        Vector3 playerPosition = player.transform.position;
        Vector3 spawnPosition = new Vector3(playerPosition.x + (tileOffset * direction), playerPosition.y, playerPosition.z);

        Instantiate(enemy, spawnPosition, Quaternion.identity);
    }
}
