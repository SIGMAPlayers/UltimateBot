
using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    public class FireWall : Strategy
    {
        public override void AssignPiratesToParticipants(List<Pirate> pirates)
        {
            List<Pirate> newPirates = new List<Pirate>(pirates);
            //GameSettings.Game.Debug("AssignPiratesToParticipants (FireWall) pirates.Count = " + pirates.Count);
            List<ICommand> templist = new List<ICommand>();

            List<Pirate> enemyCarriers = new List<Pirate>();
            foreach(Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
            {
                if(enemy.HasCapsule())
                {
                    enemyCarriers.Add(enemy);
                }
            }
            foreach(Mothership mothership in GameSettings.Game.GetEnemyMotherships())
            {
                if()
            }

            if (newPirates.Count > 1 && GameSettings.Game.GetEnemyMotherships().Length == 1)
            {
                for (int i = 0; i < newPirates.Count / 2; i++)
                {
                    templist.Add(new Front(pirates[i], new FieldAnalyzer()));
                }

                newPirates.RemoveRange(0, pirates.Count / 2);

                for (int i = 0; i < newPirates.Count; i++)
                {
                    templist.Add(new Backup(newPirates[i], new FieldAnalyzer()));
                }
            }
            else if (GameSettings.Game.GetEnemyMotherships().Length > 1)
            {
                foreach (Pirate pirate in newPirates)
                {
                    templist.Add(new Backup(pirate, new FieldAnalyzer()));
                }
            }
            else
            {
                if (newPirates.Count == 1)
                {
                    templist.Add(new Backup(newPirates[0], new FieldAnalyzer()));
                }
            }

            Participants = templist;
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
           
            
            List<BaseDefender> baseDefenders = Participants.Cast<BaseDefender>().ToList();

            foreach(BaseDefender defender in baseDefenders)
            {
                foreach (Asteroid asteroid in GameSettings.Game.GetLivingAsteroids())
                {
                    Mothership mothership = FieldAnalyzer.FindClosestMotherShip(defender.Pirate);
                    if(mothership != null)
                    {
                        if (asteroid.GetLocation().Equals(mothership))
                        {
                            //For Backup
                            int scale = (int)(asteroid.Size * 1.2);
                            if(defender is Front)
                            {
                                scale = (int)(asteroid.Size * 1.8);
                            }
                            Pirate enemyCarrier = FieldAnalyzer.GetMostThreatningEnemyCarrier(mothership);
                            defender.WhereToDefend = mothership.Location.Towards(enemyCarrier, scale);

                        }
                    }
                }
                //Backup backup = defender as Backup;
                if (defender is Backup)
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
