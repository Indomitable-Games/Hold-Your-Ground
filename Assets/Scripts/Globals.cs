using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    internal static class Globals
    {
        public static float playerSpeed = 20f;
        public static float playerTurnRadius = 30f;
        public static float playerTurnSpeed = .4f;

        public static int LastChunks = 3;

        public static int fuel = 5000;
        public static int planetID = 0;

        public static int battleDepth = -1250;
    }
}
