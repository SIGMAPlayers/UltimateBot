using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class Backup : BaseDefender
    {
        public Backup(Pirate pirate, FieldAnalyzer fieldAnalyzer) : base(pirate, fieldAnalyzer) { }

        public override Pirate Protect()
        {
            List<Mothership> motherships = GameSettings.Game.GetEnemyMotherships().ToList();
            if (motherships.Count > 0)
            {
                List<Pirate> enemiesByDistanceFromEnemyBase = GameSettings.Game.GetEnemyLivingPirates().ToList();
                enemiesByDistanceFromEnemyBase.OrderBy(Pirate => Pirate.Location.Distance(motherships[0].Location));

                int scale = 500 * 4;
                foreach (Pirate pirate in enemiesByDistanceFromEnemyBase)
                {
                    //Checks if the any of the pirates has capsule in the distance
                    //of the mothership has a capsule
                    if (pirate.Distance(motherships[0]) < scale && pirate.Capsule != null)
                        return pirate;
                }
            }

            return null;
        }


        // public override void BeHeavyInsteadOfMe(BaseAttacker me)
        // {

        // this.pirate.SwapStates(me.Pirate);
        // }


        public override void ExecuteCommand()
        {
            Pirate protectFrom = Protect();

            if (!Push())
            {
                if (protectFrom != null)
                {
                    if (GameSettings.Game.GetMyMotherships().Length == 0 && GameSettings.Game.GetMyCapsules().Length == 0)
                    {
                        Pirate.Sail(DefendAt().GetLocation());
                    }
                    else if (protectFrom.Distance(pirate) < 1000)
                    {
                        Pirate.Sail(protectFrom);
                        //Pirate.Sail(DefendAt().GetLocation());
                    }
                }
                else
                {
                    Pirate.Sail(DefendAt().GetLocation());
                }
            }
        }

        public override Location DefendAt()
        {
            Pirate enemyCarrier = null;

            if (GameSettings.Game.GetMyMotherships().Length == 0 && GameSettings.Game.GetMyCapsules().Length == 0)
            {
                Location loc = GameSettings.Game.GetEnemyMotherships()[0].GetLocation();
                loc.Row -= 100;
                return loc;
            }
            Location guardLocation;

            if (WhereToDefend == null)
            {
                foreach (Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
                {
                    if (enemy.Capsule != null)
                        enemyCarrier = enemy;
                }

                //Take care of a few citys or Ben will punch me!
                int scale = (int)(500 * 1.5);
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



                    guardLocation = GameSettings.Game.GetEnemyMotherships()[0].Location.Towards(GameSettings.Game.GetEnemyCapsules()[0], scale - 600);
                    //GameSettings.Game.Debug("Location from ProtectFromCarrier" + guardLocation);
                    return guardLocation;
                }

            }

            bool canPushWormhole = true;

            foreach (Wormhole wormhole in GameSettings.Game.GetAllWormholes())
            {
                if (wormhole.Distance(pirate) < 750)
                {
                    if (GameSettings.Game.GetEnemyMotherships().Length > 0)
                    {
                        Mothership mothership = FieldAnalyzer.FindClosestMotherShip(pirate);
                        enemyCarrier = FieldAnalyzer.GetMostThreatningEnemyCarrier(mothership);
                        if (enemyCarrier != null)
                        {
                            //Checks if the enemyPirate can come in more then 5 turns
                            if (enemyCarrier.Distance(Pirate) > enemyCarrier.MaxSpeed * 8)
                            {
                                canPushWormhole = false;
                            }
                            if (canPushWormhole)
                            {
                                guardLocation = wormhole.GetLocation().Towards(this.Pirate, wormhole.WormholeRange);
                                //    guardLocation.Col += wormhole.WormholeRange;
                                GameSettings.Game.Debug("wormhole guardLocation = " + guardLocation);
                                return guardLocation;
                            }
                        }
                    }
                }
            }

            return WhereToDefend;
        }

        /// <summary>
        /// Makes the defender try to push an enemy pirate. Returns true if it did.
        /// If can be pushed out of the map, else push againts the motherboard.
        /// </summary>
        /// <returns> true if the pirate pushed.</returns>
        public override bool Push()
        {
            foreach (Asteroid asteroid in GameSettings.Game.GetLivingAsteroids())
            {
                if (pirate.CanPush(asteroid))
                {
                    GameSettings.Game.Debug("Pirate in Backup = " + pirate);
                    GameSettings.Game.Debug("Location in Backup = " + asteroidHandler.FindBestLocationToPushTo(this.Pirate));
                    pirate.Push(asteroid, asteroidHandler.FindBestLocationToPushTo(this.Pirate));
                    return true;
                }
            }

            if (PirateToPush != null && WhereToPush != null)
            {
                if (pirate.CanPush(PirateToPush))
                {
                    pirate.Push(PirateToPush, WhereToPush);
                    return true;
                }
            }

            Pirate enemyToPush = Protect();
            if (enemyToPush != null)
            {
                if (pirate.CanPush(enemyToPush))
                {
                    //Sorry about those location numbers :\
                    //#Onemanarmy
                    if (GameSettings.Game.GetMyMotherships().Length == 0 && GameSettings.Game.GetMyCapsules().Length == 0)
                    {
                        pirate.Push(enemyToPush, new Location(3097, 1557));
                        return true;
                    }
                    Location pushTowardsOutOfMap = FieldAnalyzer.GetCloseEnoughToBorder(enemyToPush, Pirate.PushDistance);
                    if (pushTowardsOutOfMap != null)
                    {
                        pirate.Push(enemyToPush, pushTowardsOutOfMap);
                        return true;
                    }
                    else
                    {
                        if (GameSettings.Game.GetEnemyMotherships().Length > 0)
                        {
                            Location oppositeSide = enemyToPush.GetLocation().Subtract(GameSettings.Game.GetEnemyMotherships()[0].GetLocation());
                            //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                            oppositeSide = enemyToPush.GetLocation().Towards(enemyToPush.GetLocation().Add(oppositeSide), 600);
                            Pirate.Push(enemyToPush, oppositeSide);
                            //Print a message.
                            //GameSettings.Game.Debug("defender " + Pirate + " pushes " + enemyToPush + " towards " + enemyToPush.InitialLocation);
                            //Did push.
                            return true;
                        }
                        else
                        {
                            Location oppositeSide = enemyToPush.GetLocation().Subtract(new Location(0, 0));
                            //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                            oppositeSide = enemyToPush.GetLocation().Towards(enemyToPush.GetLocation().Add(oppositeSide), 600);
                            Pirate.Push(enemyToPush, oppositeSide);
                        }
                        return true;
                    }
                }
            }

            GameSettings.Game.Debug("Push distance = " + pirate.PushDistance);
            GameSettings.Game.Debug("Reached Wormhole zone");
            bool canPushWormhole = true;
            foreach (Wormhole wormhole in GameSettings.Game.GetAllWormholes())
            {
                GameSettings.Game.Debug("wormhole foreach");
                if (pirate.CanPush(wormhole))
                {
                    //GameSettings.Game.Debug("pirate.PushRange ==> " + pirate.PushRange);
                    //GameSettings.Game.Debug("pirate.MaxSpeed ==> " + pirate.MaxSpeed);
                    GameSettings.Game.Debug("wormhole.Distance(pirate)" + wormhole.Distance(pirate));

                    if (GameSettings.Game.GetEnemyMotherships().Length > 0)
                    {
                        Mothership mothership = FieldAnalyzer.FindClosestMotherShip(pirate);
                        Pirate enemyCarrier = FieldAnalyzer.GetMostThreatningEnemyCarrier(mothership);
                        if (enemyCarrier != null)
                        {
                            //Checks if the enemyPirate can come in more then 5 turns
                            GameSettings.Game.Debug("pirate.CanPush(wormhole) = " + pirate.CanPush(wormhole));
                            //Checks if the enemyPirate can come in more then 8 turns
                            if (enemyCarrier.Distance(pirate) < enemyCarrier.MaxSpeed * 8)
                            {
                                canPushWormhole = false;
                            }
                            if (canPushWormhole)
                            {
                                int cols = GameSettings.Game.Cols;
                                int rows = GameSettings.Game.Rows;
                                //Push to the center for now
                                pirate.Push(wormhole, new Location(rows / 2, cols / 2));
                                GameSettings.Game.Debug("Pirate pushed wormhole");
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
