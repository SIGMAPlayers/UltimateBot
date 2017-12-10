using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pirates;

namespace MyBot
{
    public static class GeneralMethods
    {
        /// <summary>
        /// checks if there is any threat on the given object within a given range.
        /// a threat is an enemyship who is in the range that was given
        /// </summary>
        /// <param name="mapObject"></param>
        /// <param name="rangeOfDanger"></param>
        /// <returns>true if the object is under a threat and false if it isnt</returns>
        public static Pirate UnderThreat(MapObject mapObject, int rangeOfDanger)
        {
            foreach (Pirate enemy in MyBot.game.GetEnemyLivingPirates())
            {
                if (enemy.InRange(mapObject, rangeOfDanger))
                    return enemy;
            }
            return null;
        }

        /// <summary>
        /// Makes the pirate try to push an enemy pirate. Returns true if it did.
        /// </summary>
        /// <param name="pirate">The pushing pirate.</param>
        /// <param name="game">The current game state.</param>
        /// <returns> true if the pirate pushed. </returns>
        public static bool TryPush(Pirate pirate)
        {
            // Go over all enemies.
            foreach (Pirate enemy in MyBot.game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (pirate.CanPush(enemy))
                {
                    //Changed
                    //Push enemy!
                    Location oppositeSide = enemy.GetLocation().Subtract(MyBot.game.GetEnemyMothership().GetLocation());
                    //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                    oppositeSide = enemy.GetLocation().Towards(enemy.GetLocation().Add(oppositeSide), 600);
                    pirate.Push(enemy, oppositeSide);
                    //Print a message.
                    System.Console.WriteLine("pirate " + pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);
                    //Did push.
                    return true;
                }
            }
            // Didn't push.
            return false;
        }
    }
}
