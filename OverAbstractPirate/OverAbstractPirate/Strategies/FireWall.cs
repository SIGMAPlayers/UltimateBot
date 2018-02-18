
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
            if(newPirates.Count > 1 && GameSettings.Game.GetEnemyMotherships().Length == 1)
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
            else if(GameSettings.Game.GetEnemyMotherships().Length > 1)
            {
                foreach(Pirate pirate in newPirates)
                {
                    templist.Add(new Backup(pirate, new FieldAnalyzer()));
                }
            }
            else
            {
                 if(newPirates.Count == 1)
                 {
                    templist.Add(new Backup(newPirates[0], new FieldAnalyzer()));
                 }
            }
            
            Participants = templist;
        }

        public override List<Pirate> PiratesPrioritization(List<Pirate> pirates)
        {
            List<Pirate> newlist = new List<Pirate>();
            if(GameSettings.Game.GetEnemyMotherships().Length > 0)
            {
                if(GameSettings.Game.GetMyCapsules() != null)
                {                
                    newlist = pirates.OrderBy(Pirate => Pirate.Location.Distance(GameSettings.Game.GetEnemyMotherships()[0].Location)).ToList();
                }
            }
            
            GameSettings.Game.Debug("pirates.Count PiratesPrioritization" + pirates.Count);
            return pirates;
        }

        public override void ExecuteStrategy()
        {
            foreach(BaseDefender BD in Participants.Cast<BaseDefender>().ToList())
            {
                GameSettings.Game.Debug("FireWall" + BD.Pirate.Id);
            }
            
            List<BaseDefender> baseDefenders = new List<BaseDefender>();

            if (Participants != null)
            {
                baseDefenders = Participants.Cast<BaseDefender>().ToList();
                GameSettings.Game.Debug("FireWall Participants.Count: "+Participants.Count);
            }
            
            foreach(BaseDefender defender in baseDefenders)
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
                        defender.WhereToPush = FieldAnalyzer.DefendersWhereToPush(defender.PirateToPush, defender.Pirate.PushDistance);
                    }
                }

                if(GameSettings.Game.GetEnemyMotherships().Length > 1)
                {
                    List<ICommand> fakeParticipant = new List<ICommand>(Participants);

                    foreach(Mothership mothership in GameSettings.Game.GetEnemyMotherships())
                    {   
                        for (int i = 0; i < fakeParticipant.Count / GameSettings.Game.GetEnemyMotherships().Length; i++)
                        {
                            Backup backup = fakeParticipant[i] as Backup;
                            Front front = fakeParticipant[i] as Front;
                            if (backup != null)
                            {
                                GameSettings.Game.Debug("Entered backup");
                                int scale = (int)(500 * 1.5);
                                Capsule capsule = FieldAnalyzer.GetClosestEnemyCapsuleToMothership(mothership);
                                if(capsule != null)
                                {
                                    backup.WhereToDefend = mothership.Location.Towards(capsule.GetLocation(), scale - 600);
                                    GameSettings.Game.Debug("Backup WhereToDefend = " + backup.WhereToDefend);
                                }
                            }
                            else if (front != null)
                            {
                                int scale = (int)(500 * 1.5);
                                Capsule capsule = FieldAnalyzer.GetClosestEnemyCapsuleToMothership(mothership);
                                if (capsule != null)
                                {
                                    GameSettings.Game.Debug("Entered front, capsule = " + capsule.GetLocation());
                                    front.WhereToDefend = mothership.Location.Towards(capsule.GetLocation(), scale);
                                    
                                    GameSettings.Game.Debug("Front WhereToDefend = " + front.WhereToDefend);
                                }
                            }
                        }
                    }
                }

                defender.ExecuteCommand();
            }
        }
    }
}
