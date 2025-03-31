using Assets.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            new Resource("R1", "Soft stuff", 0.1f),
            new Resource("R2", "Medium stuff", 0.5f),
            new Resource("R3", "Hard stuff", 0.9f)
        };

        #endregion

        #region WorldGen Config
        public static int LastChunks = 3;

        public static int planetID = 0;

        public static int battleDepth = int.MinValue / 2; 
        #endregion
    }
}
