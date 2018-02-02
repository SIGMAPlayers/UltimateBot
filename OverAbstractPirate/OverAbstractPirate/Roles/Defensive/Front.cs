using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    public class Front : BaseDefender
    {
        //GameSettings.Game
        public Front(Pirate pirate, FieldAnalyzer fieldAnalyzer) : base(pirate, fieldAnalyzer) { }

        public override Pirate Protect()
        {
            List<Pirate> enemiesByDistanceFromEnemyBase = GameSettings.Game.GetEnemyLivingPirates().ToList();
            enemiesByDistanceFromEnemyBase.OrderBy(Pirate => Pirate.Location.Distance(GameSettings.Game.GetEnemyMotherships()[0].Location));

            int scale = pirate.PushDistance * 4;
            foreach (Pirate pirate in enemiesByDistanceFromEnemyBase)
            {
                //Checks if the any of the pirates has capsule in the distance
                //of the mothership has a capsule
                if (pirate.Distance(GameSettings.Game.GetEnemyMotherships()[0]) < scale && pirate.Capsule != null)
                    return pirate;
            }
            return null;
        }

        public override void ExecuteCommand()
        {
            Pirate protectFrom = Protect();
            
            if(protectFrom != null)
            {
                if(!Push())
                {
                    DefendAt();
                }
            }
            else
            {
                DefendAt();
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

            int scale = (int)(500 * 1.5);
            Location guardLocation;
            if (enemyCarrier != null)
            {
                guardLocation = GameSettings.Game.GetEnemyMotherships()[0].Location.Towards(enemyCarrier, scale);
                GameSettings.Game.Debug("Location from ProtectFromCarrier" + guardLocation);
                return guardLocation;
            }

            guardLocation = GameSettings.Game.GetEnemyMotherships()[0].Location.Towards(GameSettings.Game.GetEnemyCapsules()[0], scale);
            GameSettings.Game.Debug("Location from ProtectFromCarrier" + guardLocation);
            return guardLocation;
        }

        /// <summary>
        /// Makes the defender try to push an enemy pirate. Returns true if it did.
        /// If can be pushed out of the map, else push againts the motherboard.
        /// </summary>
        /// <returns> true if the pirate pushed. </returns>
        public override bool Push()
        {
            if(PirateToPush == null)
            {
                foreach (Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
                {
                    // Check if the pirate can push the enemy.
                    if (pirate.CanPush(enemy) && enemy.HasCapsule())
                    {
                        //Changed
                        //Push enemy!
                        Location outOfBorder = FieldAnalyzer.GetCloseEnoughToBorder(enemy, pirate.PushRange);
                        if (outOfBorder != null)
                        {
                            pirate.Push(enemy, outOfBorder);
                            return true;
                        }
                        else
                        {
                            Location oppositeSide = enemy.GetLocation().Subtract(GameSettings.Game.GetEnemyMotherships()[0].GetLocation());
                            //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                            oppositeSide = enemy.GetLocation().Towards(enemy.GetLocation().Add(oppositeSide), 600);
                            pirate.Push(enemy, oppositeSide);
                            //Print a message.
                            GameSettings.Game.Debug("defender " + pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);
                            //Did push.
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
