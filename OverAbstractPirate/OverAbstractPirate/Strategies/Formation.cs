
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
            Carrier carrier = new Carrier(this.Participants);
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
            if (carrier.Pirate != null)
            {
                list.Add(carrier);
            }
            foreach (BodyGuard BG in bodyguards)
            {
                if(carrier.Pirate != null)
                    BG.assignCarrier(carrier);
                list.Add(BG);
            }
            this.Participants = list;
            
        }

        public override List<Pirate> PiratesPrioritization(List<Pirate> pirates)
        {
            List<Pirate> orderedPirates = pirates.OrderBy(Pirate => Pirate.Distance(GameSettings.Game.GetMyCapsules()[0].Location)).ToList();
            return orderedPirates;
        }
        
        public override void BeforeExecute()
        {
            ExecuteStrategy();
        }

        public override void ExecuteStrategy()
        {
            
            foreach(BaseAttacker BA in Participants.Cast<BaseAttacker>().ToList())
            {
                GameSettings.Game.Debug("Formation" + BA.Pirate.Id);
            }
            FieldAnalyzer.DefineTargets(Participants.Cast<BaseAttacker>().ToList());
            FieldAnalyzer.AssignFormationLocations(Participants.Cast<BaseAttacker>().ToList());
            BaseAttacker.FormationComplete = FormUp();
            
            List<Carrier> possibleCarriers = Participants.OfType<Carrier>().ToList();
            
            if(possibleCarriers.Count > 0)
            {
                 if(Participants.OfType<Carrier>().ToList() != null)
                {
                    if(Participants.OfType<Carrier>().ToList()[0] != null)
                    {
                        Carrier carrier = Participants.OfType<Carrier>().ToList()[0];
                        List<Pirate> enemysThreating = FieldAnalyzer.UnderThreat(carrier.Pirate, carrier.Pirate.PushRange * 4, carrier.Destination);
                        if (enemysThreating.Count > 0)
                        {
                            FieldAnalyzer.PopulateEnemyTargets(enemysThreating, Participants.Cast<BaseAttacker>().ToList());
                        }
                    }
                }
            }
               

            foreach (BaseAttacker attacker in Participants)
            {
                attacker.ExecuteCommand();
            }
        }

       private bool FormUp()
       {
            int PiratesInFormation = 0;
            int PiratesInPosition = 0;
            foreach(BaseAttacker attacker in Participants)
            {
                if(attacker.Pirate.IsAlive())
                {
                    PiratesInFormation++;    
                }
                
                if (attacker.Pirate.Distance(attacker.PositionInFormation) < 10)
                {
                    PiratesInPosition++;
                }
            }
            if(Participants.OfType<Carrier>().ToList().Count > 0)
            {
                GameSettings.Game.Debug("GOD YOU ARE HERE");
                if(FieldAnalyzer.GuardiansOnCarrier(Participants.OfType<Carrier>().ToList()[0], Participants.Cast<BaseAttacker>().ToList()))
                 {
                     GameSettings.Game.Debug("GO ALREADY!!!!!!!!");
                     return true;
                 }
            }

            if (PiratesInPosition == PiratesInFormation)
                return true;
            else
                return false;
       }
    }
}
