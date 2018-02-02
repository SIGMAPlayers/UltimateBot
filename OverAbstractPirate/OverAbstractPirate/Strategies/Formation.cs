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

        public override void AssignPiratesToParticipants(List<Pirate> pirates)
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

            
            List<ICommand> list = new List<ICommand>();
            list.Add(carrier);
            foreach (BodyGuard BG in bodyguards)
            {
                list.Add(BG);
            }
            this.Participants = list;
            
        }

        public override List<Pirate> PiratesPrioritization(List<Pirate> pirates)
        {
            List<Pirate> orderedPirates = pirates.OrderBy(Pirate => Pirate.Distance(GameSettings.Game.GetMyCapsules()[0].Location)).ToList();
            return orderedPirates;
        }

        public override void ExecuteStrategy()
        {
            
                FieldAnalyzer.DefineTargets();

                bool formationComlete = FormUp();
                if (formationComlete)
                {
                    foreach(BaseAttacker attacker in Participants)
                    {
                        attacker.SailToTarget();
                    }
                }
            
        }

       private bool FormUp()
        {
            int PiratesInFormation = 0;
            int PiratesInPosition = 0;
            foreach(BaseAttacker attacker in Participants)
            {
                PiratesInFormation++;
                if (attacker.Pirate.Distance(attacker.PositionInFormation) > 10)
                {
                    attacker.SailToPosition();
                }
                else
                {
                    PiratesInPosition++;
                }

                  if (PiratesInPosition == PiratesInFormation)
                return true;
            else
                return false;
            }

            if (PiratesInPosition == PiratesInFormation)
                return true;
            else
                return false;
        }
    }
}
