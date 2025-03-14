using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System;

public static class TextureGenerator
{

    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }


    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Tilemap TilemapFromTileArr(TileBase[] tileArr, int width, int height)
    {
        Tilemap tileMap = new();
        Vector3Int[] vector3Ints = new Vector3Int[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                vector3Ints[x * width + y] = new Vector3Int(x, 1, y);
            }
        }
        tileMap.SetTiles(vector3Ints,tileArr);
        Debug.Log(tileMap.ToString());
        return tileMap;
    }
}
