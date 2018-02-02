using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class FieldAnalyzer
    {
        //My BestFriend
        Calculator calculator;


        /// <summary>
        /// checks if there is any threat on the given pirate within a given range. sorted by distance from pirate.
        /// what is a threat:
        /// -inside the range of danger.
        /// -not 300+ units behind the pirate.
        /// -coming towards me
        /// </summary>
        /// <param name="pirate"></param>
        /// <param name="rangeOfDanger"></param>
        /// <returns>list of piretes </returns>
        public List<Pirate> UnderThreat(Pirate pirate, int rangeOfDanger, Location destination)
        {
            List<Pirate> threatingPirates = new List<Pirate>();
            foreach (Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
            {
                int angleOfAttack = calculator.CalculateAngleOfAttack(pirate, enemy, destination);
                if (enemy.InRange(pirate, rangeOfDanger) && angleOfAttack > 200 || angleOfAttack < 160)
                    threatingPirates.Add(enemy);

            }
            return threatingPirates;
        }

        #region AssignFormationLocations
        /// <summary>
        ///     when called, the function evaluates the locations of the guardiens according to the formation shape using vectors
        /// </summary>
        private void AssignFormationLocations(List<BaseAttacker> participants)
        {
            Location guardiensPosition = new Location(0,0);
            foreach(BaseAttacker Role in participants)
            {
                if(Role is Carrier)
                {
                    guardiensPosition = calculator.CalculateVectorOfFormation(Role as Carrier);
                }
            }
            foreach(BaseAttacker Role in participants)
            {
                if(Role is Carrier)
                {
                    continue;
                }
                else
                {
                    Role.PositionInFormation = guardiensPosition;
                }
            }

        }
        #endregion

        public Location DefineTargets()
        {
            //implement!
        }

        /// <summary>
        /// Checks if an enemy pirate can be pushed by multiple defenders
        /// and returns the maximum push range
        /// </summary>
        /// <param name="enemyPirate"> The enemy pirate you want to push</param>
        /// <param name="defenderList"> </param>
        /// <returns>The maximum range the enemy pirate can be pushed</returns>
        public int CheckHowManyCanPush(Pirate enemyPirate, List<BaseDefender> defenderList)
        {
            int range = 0;

            foreach (BaseDefender defender in defenderList)
            {
                if (defender.Pirate.CanPush(enemyPirate))
                    range += defender.Pirate.PushRange;
            }

            return range;
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
            Location up = new Location(0, enemyPirate.Location.Col);
            Location right = new Location(enemyPirate.Location.Row, GameSettings.Game.Cols);
            Location left = new Location(enemyPirate.Location.Row, 0);
            Location down = new Location(GameSettings.Game.Rows, enemyPirate.Location.Col);
            int upDistance = enemyPirate.Distance(up);
            int rightDistance = enemyPirate.Distance(right);
            int leftDistance = enemyPirate.Distance(left);
            int downDistance = enemyPirate.Distance(down);

            if (upDistance < rightDistance && upDistance < leftDistance && upDistance < downDistance)
            {
                if (upDistance < range)
                    return up;
            }
            else if (rightDistance < upDistance && rightDistance < leftDistance && rightDistance < downDistance)
            {
                if (rightDistance < range)
                    return right;
            }
            else if (leftDistance < upDistance && leftDistance < rightDistance && leftDistance < downDistance)
            {
                if (leftDistance < range)
                    return down;
            }
            else if (downDistance < upDistance && downDistance < rightDistance && downDistance < leftDistance)
            {
                if (downDistance < range)
                    return left;
            }
            //Returns null if not close enough to a border
            return null;
        }
    }
}

