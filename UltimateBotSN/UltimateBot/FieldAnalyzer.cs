using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pirates;
namespace MyBot
{
    public class FieldAnalyzer
    {
        /// <summary>
        /// checks if there is any threat on the given pirate within a given range. sorted by distance from pirate.
        /// what is a threat:
        /// -inside the range of danger.
        /// 
        /// -not 300+ units behind the pirate.
        /// </summary>
        /// <param name="pirate"></param>
        /// <param name="rangeOfDanger"></param>
        /// <returns>list of piretes </returns>
        public List<Pirate> UnderThreat(Pirate pirate, int rangeOfDanger)
        {
            List<Pirate> threatingPirates = new List<Pirate>();
            foreach (Pirate enemy in GameSettings.game.GetEnemyLivingPirates())
            {
                if (enemy.InRange(pirate, rangeOfDanger))
                    threatingPirates.Add(enemy);

            }
            return threatingPirates;
        }

        /// <summary>
        /// calculates the angle of an attacking pirate on us ralative to the target we are sailing to. 
        /// </summary>
        /// <param name="pirate"></param>
        /// <param name="enemy"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public int CalculateAngleOfAttack(Pirate pirate, Pirate enemy, Location target)
        {

        }
    }
}
