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
        /// <param name="defenderList"> </param>
        /// <returns>The maximum range the enemy pirate can be pushed</returns>
        public List<BaseDefender> CheckHowManyDefendrsCanPushEnemyCarrier(List<Pirate> enemyCarriers, List<BaseDefender> defenders)
        {
            List<BaseDefender> canDoublePush = new List<BaseDefender>();

            foreach(Pirate enemyCarrier in enemyCarriers)
            {
                canDoublePush = new List<BaseDefender>();

                foreach (BaseDefender defender in defenders)
                {
                    if (defender.Pirate.CanPush(enemyCarrier))
                    {
                        canDoublePush.Add(defender);
                    }
                }
                if (canDoublePush.Count > 1)
                    return canDoublePush;
            }
            return null;
        }

        /// <summary>
        /// Returns how many carriers are near the Mothership and can be double pushed
        /// </summary>
        /// <param name="defenderList"></param>
        /// <returns></returns>
        public List<Pirate> HowManyCarriersNearCityCanBeDoublePushed(List<BaseDefender> defenderList)
        {
            List<Pirate> carriers = CarrierCloseToCity();
            List<Pirate> canBeDoublePushed = new List<Pirate>();
            int countCanPush = 0;

            foreach (Pirate enemyCarrier in carriers)
            {
                countCanPush = 0;

                foreach (BaseDefender defender in defenderList)
                {
                    if (defender.Pirate.CanPush(enemyCarrier))
                    {
                        countCanPush++;
                    }
                }

                if (countCanPush > 1)
                {
                    canBeDoublePushed.Add(enemyCarrier);
                }
            }

            return canBeDoublePushed;
        }

        /// <summary>
        /// Returns a list of carriers that are close to their city
        /// </summary>
        /// <returns></returns>
        private List<Pirate> CarrierCloseToCity()
        {
            List<Pirate> closeCarriers = new List<Pirate>();

            foreach(Pirate enemyPirate in GameSettings.Game.GetEnemyLivingPirates())
            {
                if(enemyPirate.HasCapsule())
                {
                    Mothership closestEnemyMotherShip = GameSettings.Game.GetEnemyMotherships().OrderBy(Mothership => Mothership.Location.Distance(pirate)).ToList()[0];
                    if (enemyPirate.Distance(closestEnemyMotherShip) < enemyPirate.PushDistance * 2)
                    {
                        closeCarriers.Add(enemyPirate);
                    }
                }
            }
            return closeCarriers;
        }

        /// <summary>
        /// Defines where the defender need to push
        /// </summary>
        /// <param name="enemyPirate"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public Location DefendersWhereToPush(Pirate enemyPirate, int range)
        {
            Location outOfBorder = GetCloseEnoughToBorder(enemyPirate, range);
            if (outOfBorder != null)
            {
                return outOfBorder;
            }
            else
            {
                Location oppositeSide = enemyPirate.GetLocation().Subtract(GameSettings.Game.GetEnemyMotherships()[0].GetLocation());
                //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                return oppositeSide = enemyPirate.GetLocation().Towards(enemyPirate.GetLocation().Add(oppositeSide), 600);
            }
        }

        public List<Pirate> GetClosestEnemyPiratesToMothership(BaseDefender defender)
        {
            List<Pirate> closestEnemyPirates = new List<Pirate>();
            Mothership closestEnemyMotherShip = GameSettings.Game.GetEnemyMotherships().OrderBy(Mothership => Mothership.Location.Distance(defender.Pirate)).ToList()[0];

            foreach (Pirate pirate in GameSettings.Game.GetEnemyLivingPirates())
            {
                if(pirate.Distance(closestEnemyMotherShip) < defender.Pirate.PushDistance * 3.5)
                {
                    closestEnemyPirates.Add(pirate);
                }
            }

            return closestEnemyPirates;
        }

        /// <summary>
        /// Checks if the enemy pirate is close enough to the border to kill him. 
        /// Returns the location that if you push it towards it, the pirate will die or null if you can't kill it.
        /// </summary>
        /// <param name="enemyPirate">The enemy pirate to be checked.</param>
        /// <param name="range">The range that will be checked if you can throw it</param>
        /// <returns>Returns the location that if you push it towards it, the pirate will die or null if you can't kill it.</returns>
        public Location GetCloseEnoughToBorder(Pirate enemyPirate, int range)
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

