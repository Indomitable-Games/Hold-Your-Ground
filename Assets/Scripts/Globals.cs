using Assets.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            new Resource("R1", "Soft stuff", 0.1f, Resources.Load("Tile\\TilePalette\\Textures-16_7") as Tile)
        };

        #endregion

        #region WorldGen Config
        public static int LastChunks = 3;

        public static int planetID = 0;

        public static int battleDepth = int.MinValue / 2;
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
                        Resources.Load("Tile\\TilePalette\\Textures-16_0") as Tile
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
                            Resources.Load("Tile\\TilePalette\\Textures-16_7") as Tile
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
                            Resources.Load("Tile\\TilePalette\\Textures-16_15") as Tile
                        ),
                        .8f
                    )
                }
            )
        };
        #endregion
    }
}
