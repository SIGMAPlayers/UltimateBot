
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <returns>list of piretes </returns>
        public static List<Pirate> UnderThreat(MapObject mapObject, int rangeOfDanger)
        {
            List<Pirate> toReturn = new List<Pirate>();
            foreach (Pirate enemy in GameSettings.game.GetEnemyLivingPirates())
            {
                if (enemy.InRange(mapObject, rangeOfDanger))
                    toReturn.Add(enemy);

            }
            return toReturn;
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
            foreach (Pirate enemy in GameSettings.game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (pirate.CanPush(enemy))
                {
                    //Changed
                    //Push enemy!
                    Location oppositeSide = enemy.GetLocation().Subtract(GameSettings.game.GetEnemyMothership().GetLocation());
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

        /// <summary>
        /// Face towards an enemy
        /// </summary>
        /// <param name="range">The range you want to be FROM the enemyPirate</param>
        /// <param name="enemyPirate">The enemy you want to face towards him</param>
        /// <param name="myPirate">The pirate thats gonna face towards the enemyPirate</param>
        /// <returns>The location you are going to face towards the enemy pirate</returns>
        public static Location FaceTo(int range, Pirate enemyPirate, Pirate myPirate)
        {
            //int scale = range
            return enemyPirate.Location.Towards(myPirate, range);
        }


        /// <summary>
        /// Checks if the enemy pirate is close enough to the border to kill him. 
        /// Returns the location that if you push it towards it, the pirate will die or null if you can't kill it.
        /// </summary>
        /// <param name="enemyPirate">The enemy pirate to be checked.</param>
        /// <param name="range">The range that will be checked if you can throw it</param>
        /// <returns>Returns the location that if you push it towards it, the pirate will die or null if you can't kill it.</returns>
        public static Location GetCloseEnoughToBorder(Pirate enemyPirate, int range)
        {
            GameSettings.game.Debug("Range is: " + range);
            Location up = new Location(0, enemyPirate.Location.Col);
            Location right = new Location(enemyPirate.Location.Row, GameSettings.game.Cols);
            Location left = new Location(enemyPirate.Location.Row, 0);
            Location down = new Location(GameSettings.game.Rows, enemyPirate.Location.Col);
            int upDistance = enemyPirate.Distance(up);
            int rightDistance = enemyPirate.Distance(right);
            int leftDistance = enemyPirate.Distance(left);
            int downDistance = enemyPirate.Distance(down);

            GameSettings.game.Debug("Up Distance = " + upDistance + ", right distance = " + rightDistance + ", left distance = " + leftDistance + ", down distance " + downDistance);

            if (upDistance < rightDistance && upDistance < leftDistance && upDistance < downDistance)
                if (upDistance < range)
                    return up;
                else if (rightDistance < upDistance && rightDistance < leftDistance && rightDistance < downDistance)
                    if (rightDistance < range)
                        return right;
                    else if (leftDistance < upDistance && leftDistance < rightDistance && leftDistance < downDistance)
                        if (leftDistance < range)
                            return down;
                        else if (downDistance < upDistance && downDistance < rightDistance && downDistance < leftDistance)
                            if (downDistance < range)
                                return left;

            //Returns null if not close enough to a border
            return null;
        }
    }
}
