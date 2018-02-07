
using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class StrategyOrganizer
    {
        private List<Pirate> piratesToDeliver;
        private List<Strategy> strategies;
        private float assignationRatio;
        private FieldAnalyzer fieldAnalyzer;

        public List<Pirate> PiratesToDeliver { get => piratesToDeliver; set => piratesToDeliver = value; }

        public StrategyOrganizer(List<Strategy> strategies)
        {
            if(GameSettings.Game.GetMyLivingPirates() != null)
            {
                PiratesToDeliver = GameSettings.Game.GetMyLivingPirates().ToList();
            }
            this.strategies = strategies;
        }

        public void SetAssignationRatio(FieldAnalyzer FieldAnalyzer)
        {
            //For now
            assignationRatio = 1 / strategies.Count;
        }

        /// <summary>
        /// Send all available pirates to every strategy
        /// </summary>
        public void DeliverPirates()
        {
            List<Pirate> currentPirates = PiratesToDeliver;
            List<Pirate> specificPiratesForAStrategy = new List<Pirate>();
            int numberOfPiratesPerStrategy = (int)(piratesToDeliver.Count() * assignationRatio);
            foreach (Strategy strategy in strategies)
            {
                currentPirates = strategy.PiratesPrioritization(PiratesToDeliver);
                specificPiratesForAStrategy = currentPirates.GetRange(0, numberOfPiratesPerStrategy);
                currentPirates.RemoveRange(0, numberOfPiratesPerStrategy);
                strategy.AssignPiratesToParticipants(specificPiratesForAStrategy);
            }
        }

    }
}
