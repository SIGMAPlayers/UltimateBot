using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    public static class GameSettings
    {
        private static List<PirateGame> game = new List<PirateGame>();
        
        public static PirateGame Game { get => game[game.Count - 1]; set => game.Add(value); }
        public static List<PirateGame> GameList { get => game;}
    }
}
