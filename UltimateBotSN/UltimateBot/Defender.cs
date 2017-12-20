using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pirates;

namespace MyBot
{
    public enum Roles { front, backup };
    public class Defender
    {
        //First layer = 1, Second layer (Backup layer) = 2
        private Roles layer;
        private Pirate pirate;

        public Defender(Pirate pirate, Roles role)
        {
            this.layer = role;
            this.pirate = pirate;
        }

        public Pirate Pirate
        {
            get
            {
                return pirate;
            }

            set
            {
                pirate = value;
            }
        }

        public bool IsAlive()
        {
            return this.pirate.IsAlive();
        }

        public Roles Layer { get => layer; set => layer = value; }

        /// <summary>
        /// Checks if an enemy pirate can be pushed by multiple defenders
        /// and returns the maximum push range
        /// </summary>
        /// <param name="enemyPirate">The enemy pirate you want to push</param>
        /// <returns>The maximum range the enemy pirate can be pushed</returns>
        public int CheckHowManyCanPush(Pirate enemyPirate)
        {
            int range = 0;

            foreach(Defender defender in GameSettings.defList)
            {
                if (defender.Pirate.CanPush(enemyPirate))
                    range += defender.Pirate.PushRange;
            }

            return range;
        }

        /// <summary>
        /// Makes the defender try to push an enemy pirate. Returns true if it did.
        /// If can be pushed out of the map, else push againts the motherboard.
        /// </summary>
        /// <param name="pirate">The pushing pirate.</param>
        /// <param name="game">The current game state.</param>
        /// <returns> true if the pirate pushed. </returns>
        public bool TryPush(Defender defender, PirateGame game)
        {
            foreach (Pirate enemy in game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (pirate.CanPush(enemy) && enemy.HasCapsule())
                {
                    //Changed
                    //Push enemy!
                    int range = CheckHowManyCanPush(enemy);
                    Location outOfBorder = GeneralMethods.GetCloseEnoughToBorder(enemy, range);
                    if (outOfBorder != null)
                    {   
                        pirate.Push(enemy, outOfBorder);
                        return true;
                    }
                    else
                    {
                        Location oppositeSide = enemy.GetLocation().Subtract(game.GetEnemyMothership().GetLocation());
                        //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                        oppositeSide = enemy.GetLocation().Towards(enemy.GetLocation().Add(oppositeSide), 600);
                        pirate.Push(enemy, oppositeSide);
                        //Print a message.
                        game.Debug("defender " + defender + " pushes " + enemy + " towards " + enemy.InitialLocation);
                        //Did push.
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Chooses a pirate in range of their city to attack.
        /// </summary>
        /// <param name="game">The current game state.</param>
        /// <returns> Returns the closest pirate to attack. </returns>
        public Pirate DefendFrom(PirateGame game)
        {
            //Carries to city - 100;
            List<Pirate> enemiesByDistanceFromEnemyBase = game.GetEnemyLivingPirates().ToList();
            enemiesByDistanceFromEnemyBase.OrderBy(Pirate => Pirate.Location.Distance(game.GetEnemyMothership().Location));
            List<Pirate> potentialThreat = new List<Pirate>
            {
                null
            };
            int scale = 500 * 4;
            foreach (Pirate pirate in enemiesByDistanceFromEnemyBase)
            {
                //Checks if the any of the pirates has capsule in the distance
                //of the mothership has a capsule
                if (pirate.Distance(game.GetEnemyMothership()) < scale && pirate.Capsule != null)
                    return pirate;
            }
            return null;
        }

        /// <summary>
        /// Generates dynamically a guard location so the defenders will always be infront
        /// of the enemyCarrier. Returns the location the defenders need to be in.
        /// </summary>
        /// <remarks>For explanation ask me, Idan, Booba or Matan Meushar</remarks>
        /// <param name="range">The range from the first layer range = 0 (first) range = ~450 (second).</param>
        /// <param name="game">The current game state.</param>
        /// <returns> Returns the basic location for the defenders. </returns>
        /// Changed to work with two layers
        public Location ProtectFromCarriers(int range, PirateGame game)
        {
            Pirate enemyCarrier = null;

            foreach (Pirate enemy in game.GetEnemyLivingPirates())
            {
                if (enemy.Capsule != null)
                    enemyCarrier = enemy;
            }

            int scale = (int)(500 * 1.5);
            Location guardLocation;
            if (enemyCarrier != null)
            {
                guardLocation = game.GetEnemyMothership().Location.Towards(enemyCarrier, scale - range);
                game.Debug("Location from ProtectFromCarrier" + guardLocation);
                return guardLocation;
            }

            guardLocation = game.GetEnemyMothership().Location.Towards(game.GetEnemyCapsule(), scale - range);
            game.Debug("Location from ProtectFromCarrier" + guardLocation);
            return guardLocation;
        }
    }
}
