using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    public static class GameSettings
    {
        private static List<PirateGame> game = new List<PirateGame>();
        
        private static List<Location> lastGameEnemyPirates = new  List<Location>();
        private static List<Location> lastGameMyLivingPirates = new  List<Location>();
        private static List<Location> lastGameLivingAsteroids = new  List<Location>();
        private static List<Location> lastGameWormholes = new  List<Location>();
        
        public static void resetLists()
        {

            foreach (Pirate p in Game.GetAllMyPirates())
            {
                lastGameMyLivingPirates.Add(p.InitialLocation);
            }

            foreach (Pirate p in Game.GetAllEnemyPirates())
            {
                lastGameEnemyPirates.Add(p.InitialLocation);
            }

            foreach (Asteroid a in Game.GetAllAsteroids())
            {
                lastGameLivingAsteroids.Add(a.InitialLocation);
            }

            foreach (Wormhole w in Game.GetAllWormholes())
            {
                lastGameWormholes.Add(w.InitialLocation);
            }
        }

        public static PirateGame Game { get => game[game.Count - 1]; set => game.Add(value); }
        //public static List<PirateGame> GameList { get => game;}
        public static List<Location>  LastGameEnemyPirates { get => lastGameEnemyPirates; }
        public static List<Location>  LastGameMyLivingPirates { get => lastGameMyLivingPirates; }
        public static List<Location>  LastGameLivingAsteroids { get => lastGameLivingAsteroids; }
        public static List<Location>  LastGameWormholes { get => lastGameWormholes; }
        
        
        public static void SetLastGame()
        {
        
            // public static void SetLastGameEnemyPirates()
            
            if (Game.Turn == 1)
            {
                resetLists();
            }
            
            // if (Game.GetEnemyLivingPirates().Length > 0)
            //     lastGameEnemyPirates = Game.GetEnemyLivingPirates().ToList();
            Location enemyloc;
            foreach (Pirate enemy in Game.GetEnemyLivingPirates())
            {
                enemyloc = new Location(enemy.Location.Row , enemy.Location.Col);
                lastGameEnemyPirates[enemy.Id] = enemyloc;
            }
            
            Location pirateloc;
            foreach (Pirate pirate in Game.GetMyLivingPirates())
            {
                pirateloc = new Location(pirate.Location.Row , pirate.Location.Col);
                lastGameMyLivingPirates[pirate.Id] = pirateloc;
            }
                    
            foreach (Asteroid asteroid in Game.GetLivingAsteroids())
            {
                lastGameLivingAsteroids[asteroid.Id] = asteroid.Location;
            }
                
            foreach (Wormhole wormhole in Game.GetAllWormholes())
            {
                lastGameWormholes[wormhole.Id] = wormhole.Location;
            }  
        }
    }
}
