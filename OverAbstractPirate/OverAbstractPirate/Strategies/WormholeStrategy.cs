
using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    public class WornholeStrategy : Strategy
    {
        public override void AssignPiratesToParticipants(List<Pirate> pirates)
        {
            List<ICommand> members = new List<ICommand>();
            members.Add(new King(pirates[0]));
        }

        public override List<Pirate> PiratesPrioritization(List<Pirate> pirates)
        {
            List<Pirate> list = pirates;
            list = list.OrderBy(Pirate => Pirate.Distance(GameSettings.Game.GetMyCapsules().ToList()[0])).ToList();
            return list;

        }

        public override void ExecuteStrategy()
        {

        }
    }
}