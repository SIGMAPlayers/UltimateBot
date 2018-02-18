using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    class MyBot : IPirateBot
    {
        List<Strategy> strategies;
        
        FieldAnalyzer FA;
        StrategyOrganizer strategyOrganizer;
        
        public void DoTurn(PirateGame game)
        {
            GameSettings.Game = game;
            
            /*if (GameSettings.Game.GetMyLivingPirates().Length > 0  &&  GameSettings.LastGameLivingAsteroids.Count > 1)
            {
                GameSettings.Game.Debug("pirate previous location: "+GameSettings.LastGameMyLivingPirates[0]+" *** current location: "+GameSettings.Game.GetMyLivingPirates()[0].Location);
                GameSettings.Game.Debug("asteroid 1 previous location: "+GameSettings.LastGameLivingAsteroids[1]+" *** current location: "+GameSettings.Game.GetLivingAsteroids()[1].Location);
            } */  


            if(GameSettings.Game.Turn == 1)
            {
                List<Strategy> tmp = new List<Strategy>();
                if(GameSettings.Game.GetMyCapsules().ToList().Count > 0)
                {
                    tmp.Add(new Formation());
                }
                if(GameSettings.Game.GetEnemyMotherships().ToList().Count > 0)
                {
                    tmp.Add(new FireWall());
                }
                strategies = tmp;
                strategyOrganizer = new StrategyOrganizer(strategies);
                FA = new FieldAnalyzer();
                 
            }
           
            
            strategyOrganizer.DeliverPirates();
        
           
            

            foreach (Strategy strategy in strategies)
            {
                strategy.ExecuteStrategy();
            }
            
            
            //update last turn objects locations
            GameSettings.SetLastGame();
            
            
            // GameSettings.SetLastGameMyLivingPirates();
            // GameSettings.SetLastGameEnemyPirates();
            // GameSettings.SetLastGameLivingAsteroids();
            
            // if (GameSettings.Game.GetAllWormholes().Count > 0)
            //     GameSettings.SetLastGameWormholes();
        }
    }
}
