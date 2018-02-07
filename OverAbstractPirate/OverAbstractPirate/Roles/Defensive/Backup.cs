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
            List<Pirate> enemiesByDistanceFromEnemyBase = GameSettings.Game.GetEnemyLivingPirates().ToList();
            enemiesByDistanceFromEnemyBase.OrderBy(Pirate => Pirate.Location.Distance(GameSettings.Game.GetEnemyMotherships()[0].Location));
            List<Pirate> potentialThreat = new List<Pirate>
            {
                null
            };
            int scale = 500 * 4;
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

            if (protectFrom != null)
            {
                if (!Push())
                {
                    pirate.Sail(DefendAt().GetLocation());
                }
            }
            else
            {
                pirate.Sail(DefendAt().GetLocation());
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
                guardLocation = GameSettings.Game.GetEnemyMotherships()[0].Location.Towards(enemyCarrier, scale - 450);
                //GameSettings.Game.Debug("Location from ProtectFromCarrier" + guardLocation);
                return guardLocation;
            }

            guardLocation = GameSettings.Game.GetEnemyMotherships()[0].Location.Towards(GameSettings.Game.GetEnemyCapsules()[0], scale - 450);
            //GameSettings.Game.Debug("Location from ProtectFromCarrier" + guardLocation);
            return guardLocation;
        }

        /// <summary>
        /// Makes the defender try to push an enemy pirate. Returns true if it did.
        /// If can be pushed out of the map, else push againts the motherboard.
        /// </summary>
        /// <returns> true if the pirate pushed.</returns>
        public override bool Push()
        {
            if (PirateToPush != null && WhereToPush != null)
            {
                if (pirate.CanPush(PirateToPush))
                {
                    pirate.Push(PirateToPush, WhereToPush);
                    return true;
                }
            }
            return false;
        }
    }
}
