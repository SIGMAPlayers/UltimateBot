
using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    public class FireWall : Strategy
    {
        public override void AssignPiratesToParticipants(List<Pirate> pirates)
        {
            for (int i = 0; i < pirates.Count / 2; i++)
            {
                Participants[i] = new Front(pirates[i], new FieldAnalyzer());
            }

            pirates.RemoveRange(0, pirates.Count / 2);

            for (int i = 0; i < pirates.Count; i++)
            {
                Participants[i] = new Backup(pirates[i], new FieldAnalyzer());
            }

            foreach(ICommand item in Participants)
            {
                BaseDefender miniItem = item as BaseDefender;
                GameSettings.Game.Debug("List of FireWall: " + miniItem.Pirate.Id);
            }
        }

        public override List<Pirate> PiratesPrioritization(List<Pirate> pirates)
        {
            if(GameSettings.Game.GetEnemyMotherships().Length > 0)
            {
                pirates = GameSettings.Game.GetAllMyPirates().OrderBy(Pirate => Pirate.Location.Distance(GameSettings.Game.GetMyCapsules()[0].Location)).ToList();
            }

            return pirates;
        }

        public override void ExecuteStrategy()
        {
            List<BaseDefender> baseDefenders = new List<BaseDefender>();

            if (Participants != null)
            {
                baseDefenders = Participants.Cast<BaseDefender>().ToList();
            }

            GameSettings.Game.Debug("Participants.Count: "+Participants.Count);

            foreach (BaseDefender defender in baseDefenders)
            {
                //Backup backup = defender as Backup;
                if(defender is Backup)
                {
                    List<Pirate> enemyCarriers = FieldAnalyzer.HowManyCarriersNearCityCanBeDoublePushed(baseDefenders);
                    if (enemyCarriers.Count > 0)
                    {
                        enemyCarriers = enemyCarriers.OrderBy(Pirate => Pirate.Location.Distance(defender.Pirate)).ToList();
                        defender.PirateToPush = enemyCarriers[0];
                        List<Pirate> pirateToSend = new List<Pirate>() { defender.PirateToPush };
                        defender.WhereToPush = FieldAnalyzer.DefendersWhereToPush(defender.PirateToPush, FieldAnalyzer.CheckHowManyDefendrsCanPushEnemyCarrier(pirateToSend, baseDefenders).Count * defender.Pirate.PushDistance);
                    }
                }
                else
                { 
                    List<Pirate> threatingEnemies = FieldAnalyzer.GetClosestEnemyPiratesToMothership(defender);

                    List<BaseDefender> defenderToSend = new List<BaseDefender> { defender };
                    List<BaseDefender> multipleDefendersCanPushIt = FieldAnalyzer.CheckHowManyDefendrsCanPushEnemyCarrier(GameSettings.Game.GetEnemyLivingPirates().ToList(), defenderToSend);
                    if(multipleDefendersCanPushIt.Count > 1)
                    {
                        defender.PirateToPush = multipleDefendersCanPushIt[0].Pirate;
                    }
                    defender.WhereToPush = FieldAnalyzer.DefendersWhereToPush(defender.PirateToPush, defender.Pirate.PushDistance);
                }

                defender.ExecuteCommand();
            }
        }
    }
}
