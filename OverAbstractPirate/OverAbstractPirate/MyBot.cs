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
           
            strategyOrganizer.SendStrategyToCommunicator();
            strategyOrganizer.DeliverPirates();
        
           
            

            foreach (Strategy strategy in strategies)
            {
                strategy.BeforeExecute();
            }
        }
    }
}
