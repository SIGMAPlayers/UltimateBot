using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class DeathBallDefender : BaseDefender
    {
        private Asteroid pushTowards;

        public Asteroid PushTowards { get => pushTowards; set => pushTowards = value; }

        public DeathBallDefender(Pirate pirate) : base(pirate, new FieldAnalyzer())
        {
            foreach (Asteroid asteroid in GameSettings.Game.GetLivingAsteroids())
            {
                Mothership closestMothership = FieldAnalyzer.GetClosestEnemyMothership(pirate);

                if (closestMothership != null)
                {
                    if (asteroid.GetLocation().Equals(closestMothership.GetLocation()))
                    {
                        pushTowards = asteroid;
                    }
                }
            }
        }

        public override Location DefendAt()
        {
            Pirate enemyCarrier = null;

            foreach (Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
            {
                if (enemy.Capsule != null)
                    enemyCarrier = enemy;
            }

            if (WhereToDefend == null)
            {
                //Take care of a few citys or Ben will punch me!
                int scale = (int)(500 * 1.5);
                Location guardLocation;
                if (GameSettings.Game.GetEnemyMotherships().Length > 0)
                {

                    if (enemyCarrier != null)
                    {
                        if (enemyCarrier.Distance(pirate) > enemyCarrier.MaxSpeed * 8)
                        {
                            //guardLocation = GameSettings.Game.GetEnemyMotherships()[0].Location.Towards(enemyCarrier, scale - 450);
                            guardLocation = GameSettings.Game.GetEnemyMotherships()[0].Location.Towards(enemyCarrier, scale - 600);
                            //GameSettings.Game.Debug("Location from ProtectFromCarrier" + guardLocation);
                            return guardLocation;
                        }
                    }

                    bool canPushWormhole = true;

                    foreach (Wormhole wormhole in GameSettings.Game.GetAllWormholes())
                    {
                        if (wormhole.Distance(pirate) < 750)
                        {
                            foreach (Pirate enemyPirate in GameSettings.Game.GetEnemyLivingPirates())
                            {
                                //Checks if the enemyPirate can come in more then 5 turns
                                if (enemyPirate.Distance(wormhole) < enemyPirate.MaxSpeed * 5)
                                {
                                    canPushWormhole = false;
                                }
                                if (canPushWormhole)
                                {
                                    guardLocation = wormhole.GetLocation();
                                    GameSettings.Game.Debug("wormhole guardLocation = " + guardLocation);
                                    return guardLocation;
                                }
                            }
                        }
                    }

                    guardLocation = GameSettings.Game.GetEnemyMotherships()[0].Location.Towards(GameSettings.Game.GetEnemyCapsules()[0], scale - 600);
                    //GameSettings.Game.Debug("Location from ProtectFromCarrier" + guardLocation);
                    return guardLocation;
                }
            }

            return new Location(0, 0);
        }

        public override void ExecuteCommand()
        {
            if (!Push())
            {
                Location loc = DefendAt();
                Pirate.Sail(loc);
                GameSettings.Game.Debug("DeathBallDefender location = " + loc);
            }
        }

        public override Pirate Protect()
        {
            List<Pirate> enemiesByDistanceFromEnemyBase = GameSettings.Game.GetEnemyLivingPirates().ToList();
            enemiesByDistanceFromEnemyBase.OrderBy(Pirate => Pirate.Location.Distance(GameSettings.Game.GetEnemyMotherships()[0].Location));

            int scale = Pirate.PushDistance * 4;
            foreach (Pirate pirate in enemiesByDistanceFromEnemyBase)
            {
                //Checks if the any of the pirates has capsule in the distance
                //of the mothership has a capsule
                if (pirate.Distance(GameSettings.Game.GetEnemyMotherships()[0]) < scale && pirate.Capsule != null)
                    return pirate;
            }
            return null;
        }

        public override bool Push()
        {
            foreach (Asteroid asteroid in GameSettings.Game.GetLivingAsteroids())
            {//guy is stupid
                Mothership closestMothership = FieldAnalyzer.GetClosestEnemyMothership(pirate);
                if (closestMothership != null)
                {
                    GameSettings.Game.Debug("Before asteroid.GetLocation().InRange(closestMothership, 100)");
                    if (asteroid.GetLocation().InRange(closestMothership, 100))
                    {
                        if (pirate.CanPush(asteroid))
                        {
                            pirate.Push(asteroid, asteroid);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
