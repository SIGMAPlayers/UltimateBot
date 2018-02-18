using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class DeathBall : Strategy
    {
        public override void AssignPiratesToParticipants(List<Pirate> pirates)
        {
            List<Pirate> newPirates = new List<Pirate>(pirates);
            //GameSettings.Game.Debug("AssignPiratesToParticipants (FireWall) pirates.Count = " + pirates.Count);
            List<ICommand> templist = new List<ICommand>();
            Asteroid minDistanceAsteroid = null;
            Pirate minDistancePirate = null;
            int minDistance = 100000;

            foreach (Asteroid asteroid in GameSettings.Game.GetLivingAsteroids())
            {
                foreach (Pirate pirate in GameSettings.Game.GetMyLivingPirates())
                {
                    if (asteroid.Distance(pirate) < minDistance)
                    {
                        minDistance = asteroid.Distance(pirate);
                        minDistanceAsteroid = asteroid;
                        minDistancePirate = pirate;
                    }
                }
            }


            if (minDistancePirate != null)
            {
                templist.Add(new DeathBallAttacker(minDistancePirate));
            }

            newPirates.Remove(minDistancePirate);

            for (int i = 0; i < newPirates.Count; i++)
            {
                templist.Add(new DeathBallDefender(newPirates[i]));
            }

            /*Mothership minDistanceMothership = null;
            Pirate minDistancePirate = null;
            int minDistance = 100000;
            foreach (Mothership mothership in GameSettings.Game.GetMyMotherships())
            {
                foreach (Pirate pirate in GameSettings.Game.GetMyLivingPirates())
                {
                    if (mothership.Distance(pirate) < minDistance)
                    {
                        minDistance = mothership.Distance(pirate);
                        minDistanceMothership = mothership;
                        minDistancePirate = pirate;
                    }
                }
            }
            
            if (minDistancePirate != null)
            {
                templist.Add(new DeathBallDefender(minDistancePirate));
            }
            
            newPirates.Remove(minDistancePirate);

            for (int i = 0; i < newPirates.Count; i++)
            {
                templist.Add(new DeathBallAttacker(newPirates[i]));
            }*/



            Participants = templist;
        }

        public override List<Pirate> PiratesPrioritization(List<Pirate> pirates)
        {
            return pirates;
        }

        public override void ExecuteStrategy()
        {
            DeathBallAttacker pusher = null;
            List<DeathBallDefender> baseDefenders = new List<DeathBallDefender>();

            List<ICommand> participants = new List<ICommand>(Participants);

            foreach (ICommand pirate in participants)
            {
                if (pirate is DeathBallAttacker)
                {
                    pusher = pirate as DeathBallAttacker;
                }
            }

            participants.Remove(pusher);

            foreach (BaseDefender BD in participants.Cast<BaseDefender>().ToList())
            {
                GameSettings.Game.Debug("DeathBall = " + BD.Pirate.Id);
            }

            if (participants != null)
            {
                baseDefenders = participants.Cast<DeathBallDefender>().ToList();
            }

            Asteroid dodge = null;

            foreach (DeathBallDefender defender in baseDefenders)
            {
                foreach (Asteroid asteroid in GameSettings.Game.GetLivingAsteroids())
                {
                    Mothership closestMothership = FieldAnalyzer.GetClosestEnemyMothership(defender.Pirate);

                    if (closestMothership != null)
                    {
                        if (asteroid.GetLocation().InRange(closestMothership, 500))
                        {
                            dodge = asteroid;
                        }
                    }
                }


                if (pusher.PushedAsteroid && dodge != null)
                {
                    GameSettings.Game.Debug("Asteroid DeathBall is: " + dodge.Location);
                    defender.WhereToDefend = dodge.GetLocation().Towards(defender.Pirate, dodge.Size + 50);
                }
            }

            foreach (ICommand bot in Participants)
            {
                bot.ExecuteCommand();
            }
        }
    }
}
