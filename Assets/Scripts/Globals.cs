using Assets.Scripts.Objects;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    internal static class Globals
    {
        #region PlayerStats

        public static float playerSpeed = 20f;
        public static float playerTurnRadius = 30f;
        public static float playerTurnSpeed = .4f;

        public static int fuel = 5000;

        public static Resource[] playerResources = {
            new Resource("R1", "Soft stuff", 0.1f, Resources.Load("Tile\\TilePalette\\BaseTiles\\Base_Stone_0") as Tile)
        };

        #endregion

        #region WorldGen Config
        public static int LastChunks = 3;

        public static int planetID = 0;

        public static int battleDepth = -20000;
        #endregion

        #region shop
        public static int shopIndex = 0;

        public static List<List<Vector2Int[]>> shop = new List<List<Vector2Int[]>>()
        {
            new List<Vector2Int[]>
            {
                new Vector2Int[]
                {
                    new Vector2Int (2,3),
                    new Vector2Int (0,0)
                },
                new Vector2Int[]
                {
                    new Vector2Int (2,3),
                    new Vector2Int (4,4)
                }
            },
            new List<Vector2Int[]>
            {
                new Vector2Int[]
                {
                    new Vector2Int (2,2),
                    new Vector2Int (0,0)
                },
                new Vector2Int[]
                {
                    new Vector2Int (2,3),
                    new Vector2Int (2,2)
                }
            }
        };
        #endregion

        #region Planets

        public static List<Planet> Planets = new List<Planet>()
        {
            new Planet
            (
                1455,
                .03f,
                FastNoiseLite.NoiseType.OpenSimplex2,
                FastNoiseLite.FractalType.None,
                4,
                2,
                .5f,
                .05f,
                2,
                FastNoiseLite.CellularDistanceFunction.EuclideanSq,
                FastNoiseLite.CellularReturnType.Distance,
                1,
                FastNoiseLite.DomainWarpType.OpenSimplex2,
                2.12f,
                .9f,
                new List<Resource>()
                {
                    new Resource
                    (
                        "test",
                        "test again",
                        1.2f,
                        Resources.Load<Tile>("Tile\\TilePalette\\BaseTiles\\Base_Stone_0")
                    ),
                    new Resource
                    (
                        "test",
                        "test again",
                        1.2f,
                        Resources.Load<Tile>("Tile\\TilePalette\\BaseTiles\\Base_Stone_7")
                    ),
                    new Resource
                    (
                        "test",
                        "test again",
                        1.2f,
                        Resources.Load<Tile>("Tile\\TilePalette\\BaseTiles\\Base_Stone_14")
                    )
                },
                new List<Ore>()
                {
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Coal-1") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Coal-2") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Copper-1") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Copper-2") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Diamond-1") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Diamond-2") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Gold-1") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Gold-2") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Iron-1") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Iron-2") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Lapis-1") as Tile
                        ),
                        .8f
                    ),
                    new Ore
                    (
                        new Resource
                        (
                            "test",
                            "test agains",
                            .2f,
                            Resources.Load("Tile\\TilePalette\\OreTiles\\Lapis-2") as Tile
                        ),
                        .8f
                    )
                }
            )
        };
        #endregion
    }
}
