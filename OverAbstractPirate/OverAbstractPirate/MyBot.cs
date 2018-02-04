
using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    class MyBot : IPirateBot
    {
        List<Strategy> strategies = new List<Strategy>()
        {
            new Formation(),
            new FireWall()
        };
        
        public void DoTurn(PirateGame game)
        {
            if (game.Turn == 1)
            {
                StrategyOrganizer strategyOrganizer = new StrategyOrganizer(strategies);
                strategyOrganizer.DeliverPirates();
            }

            foreach (Strategy strategy in strategies)
            {
                strategy.ExecuteStrategy();
            }
        }
    }
}
