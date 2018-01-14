using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pirates;

namespace MyBot
{
    public static class GameSettings
    {
        public static AllPirates allPirates;
        public static DefenderList defList;
        public static AttackerList AtkList;
        public static PirateGame game;
        public const int FORMATION_COUNT = 1;
        public const int OFFSET_X = 300;
        public const int OFFSET_Y = 300;
        public static bool START = false;
    }

}
