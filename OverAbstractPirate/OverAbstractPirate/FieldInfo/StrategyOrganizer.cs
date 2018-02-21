using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class StrategyOrganizer
    {
        private List<Pirate> piratesToDeliver;
        private List<Strategy> strategies;
        private double assignationRatio;
        private FieldAnalyzer fieldAnalyzer;
        private List<List<Pirate>> PiratesForEveryStrategy;

        public List<Pirate> PiratesToDeliver { get => piratesToDeliver; set => piratesToDeliver = value; }

        public StrategyOrganizer(List<Strategy> strategies)
        {
            PiratesToDeliver = GameSettings.Game.GetMyLivingPirates().ToList();
            this.strategies = strategies;
            fieldAnalyzer = new FieldAnalyzer();
            
            PiratesForEveryStrategy = new List<List<Pirate>>();
            GameSettings.Game.Debug("Strategies Count == "+ strategies.Count);
        }

        public void SetAssignationRatio()
        {
            //For now
            assignationRatio = 1.0 / (double)strategies.Count;
        }
        
        public void Prioritizer()
        {
            SetAssignationRatio();
            List<Pirate> currentPirates = PiratesToDeliver;
            List<Pirate> specificPiratesForAStrategy = new List<Pirate>();
            int numberOfPiratesPerStrategy = (int)(piratesToDeliver.Count()*assignationRatio);
            
            foreach(Strategy s in strategies)
            {
                currentPirates = s.PiratesPrioritization(currentPirates);
                specificPiratesForAStrategy = currentPirates.GetRange(0, numberOfPiratesPerStrategy);
                PiratesForEveryStrategy.Add(specificPiratesForAStrategy);
                currentPirates.RemoveRange(0, numberOfPiratesPerStrategy);
            }
            
            if(currentPirates.Count > 0)
            {
                foreach(Pirate p in currentPirates)
                {
                    PiratesForEveryStrategy[0].Add(p);
                }
            }
        }
        public void Assigner()
        {
            for(int i = 0; i < strategies.Count; i++)
            {
                strategies[i].AssignPiratesToParticipants(PiratesForEveryStrategy[i]);
            }
        }
        
         public void SendStrategyToCommunicator()
        {
            Communicator c = Communicator.GetInstance();
            c.StratList = strategies;
        }

        /// <summary>
        /// Send all available pirates to every strategy
        /// </summary>
        public void DeliverPirates()
        {     
            //Has 8 pirates
            List<Pirate> currentPirates = PiratesToDeliver;
            List<Pirate> specificPiratesForAStrategy = new List<Pirate>();
            int numberOfPiratesPerStrategy = (int)(piratesToDeliver.Count()*assignationRatio);
            
           
            if(PiratesToDeliver.Count > 0)
            {
                GameSettings.Game.Debug("piratesToDeliver.Count()*assignationRatio = " + numberOfPiratesPerStrategy);
                if(PiratesForEveryStrategy.Count == 0)
                {
                    Prioritizer();    
                }
                
                Assigner();
                
            }
           
        }
    }
}
