using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    public class MyBot : IPirateBot
    {
        List<Strategy> strategies;

        FieldAnalyzer FA;
        StrategyOrganizer strategyOrganizer;

        public void DoTurn(PirateGame game)
        {
            GameSettings.Game = game;

            List<Strategy> tmp = new List<Strategy>();

            if (game.GetLivingAsteroids().Length > 0 && game.GetEnemyMotherships().Length == 1)
            {
                tmp.Add(new DeathBall());
            }
            else if (GameSettings.Game.GetEnemyMotherships().ToList().Count > 0)
            {
                tmp.Add(new FireWall());
            }
            if (GameSettings.Game.GetMyCapsules().ToList().Count > 0)
            {
                tmp.Add(new Formation());
            }

            strategies = tmp;
            strategyOrganizer = new StrategyOrganizer(strategies);
            FA = new FieldAnalyzer();


            strategyOrganizer.DeliverPirates();




            foreach (Strategy strategy in strategies)
            {
                strategy.ExecuteStrategy();
            }
        }
    }
}
