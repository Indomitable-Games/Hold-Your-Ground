using UnityEngine;
using System.Collections;
using static UnityEngine.Mesh;
using UnityEngine.Tilemaps;

public class MapDisplay : MonoBehaviour
{

    public Tilemap tileMap;

    public void DrawTexture(Texture2D texture)
    {
        
    }
    public void DrawTileMap(Tilemap tileMap)
    {
        this.tileMap = tileMap;
    }
}
