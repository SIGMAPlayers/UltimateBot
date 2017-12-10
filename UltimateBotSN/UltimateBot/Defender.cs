using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pirates;

namespace MyBot
{
    public enum Roles { front, backup };

    public class Defender : Pirate
    {
        //First layer = 1, Second layer (Backup layer) = 2
        private Roles layer;

        public Roles Layer { get => layer; set => layer = value; }

        public Defender(Roles layer) : base()
        {
            this.Layer = layer;
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
                if (defender.CanPush(enemy) && enemy.HasCapsule())
                {
                    //Changed
                    //Push enemy!
                    Location outOfBorder = MyBot.GetCloseEnoughToBorder(defender, defender.PushDistance);
                    if (outOfBorder != null)
                    {
                        defender.Push(enemy, outOfBorder);
                        return true;
                    }
                    else
                    {
                        Location oppositeSide = enemy.GetLocation().Subtract(game.GetEnemyMothership().GetLocation());
                        //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                        oppositeSide = enemy.GetLocation().Towards(enemy.GetLocation().Add(oppositeSide), 600);
                        defender.Push(enemy, oppositeSide);
                        //Print a message.
                        System.Console.WriteLine("defender " + defender + " pushes " + enemy + " towards " + enemy.InitialLocation);
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
        // Changed Pirates to defenders where needed
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
            //Try to use only 
            //game.GetEnemyCapsule().Location.Towards(game.GetEnemyMothership(), scale - range);
            //To follow the enemy carrier/the capsule
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
                guardLocation = enemyCarrier.Location.Towards(game.GetEnemyMothership(), scale - range);
                return guardLocation;
            }

            guardLocation = game.GetEnemyCapsule().Location.Towards(game.GetEnemyMothership(), scale - range);

            return guardLocation;
        }




    }
}
