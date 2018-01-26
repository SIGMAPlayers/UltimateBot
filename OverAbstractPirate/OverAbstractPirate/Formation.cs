using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;


namespace MyBot
{
    public class Formation : Strategy
    {
        
        public Formation()
        {
        }

        protected override void AssignPiratesToParticipants(List<Pirate> pirates)
        {
            Carrier carrier = new Carrier();
            List<BodyGuard> bodyguards = new List<BodyGuard>();

            foreach(Pirate pirate in pirates)
            {
                if(pirate.HasCapsule() && carrier.Pirate == null)
                {
                    carrier.Pirate = pirate;
                }
                else
                {
                    bodyguards.Add(new BodyGuard(pirate));
                }
            }

            this.Participants.Add(carrier);          
            foreach(BodyGuard BG in bodyguards)
            {
                this.Participants.Add(BG);
            }
            
        }

        protected override List<Pirate> PiratesPrioritization(List<Pirate> pirates)
        {
            List<Pirate> orderedPirates = pirates.OrderBy(Pirate => Pirate.Distance(GameSettings.Game.GetMyCapsule().Location)).ToList();
            return orderedPirates;
        }

        protected override void ExecuteStrategy()
        {
            throw new NotImplementedException();
        }

       
    }
}
