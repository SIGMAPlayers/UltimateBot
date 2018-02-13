using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public abstract class Strategy
    {
        private List<ICommand> participants;
        private FieldAnalyzer fieldAnalyzer;
        
        public Strategy()
        {
            fieldAnalyzer = new FieldAnalyzer();
            participants = new List<ICommand>();
        }

        /// <summary>
        /// list of the Assigned Roles
        /// </summary>
        public List<ICommand> Participants { get => participants; set => participants = value;}
        /// <summary>
        /// FieldAnalyzer of that strategy
        /// </summary>
        protected FieldAnalyzer FieldAnalyzer { get => fieldAnalyzer; set => fieldAnalyzer = value; }

        /// <summary>
        /// priorites a given list of pirates according the self needs.
        /// exp. Formation of attackers will prefer those who are the closest to the Capsule.
        /// </summary>
        /// <param name="pirates">list to priorites.</param>
        /// <returns>prioritiesed list</returns>
        public abstract List<Pirate> PiratesPrioritization(List<Pirate> pirates);

        /// <summary>
        /// Assigns the given pirates each to his role (set of commands) and pushes him to the participants list.
        /// </summary>
        /// <param name="pirates">the pirate that are going to take part in this strategy</param>
        public abstract void AssignPiratesToParticipants(List<Pirate> pirates);

        /// <summary>
        /// the main mind of the strategy. uses the FieldAnalyzer to make a decision
        /// and send a command to each sub-role in the participants list
        /// </summary>
        public abstract void ExecuteStrategy();

       


    }
}
