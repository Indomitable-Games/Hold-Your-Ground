using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleGen : MonoBehaviour
{

    public GameObject enemy;
    private GameObject player;
    public Tilemap world;

    private static System.Random rand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rand = new System.Random();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the Player GameObject has the correct tag.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
