using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;
namespace MyBot
{
    public class Calculator
    {
        public Calculator()
        {
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
            //crate 2 vectors
            Location U = target.Subtract(pirate.Location);//from pirate to target
            Location V = enemy.Location.Subtract(pirate.Location);//from pirate to enemy

            double cosAngle = ((U.Col * V.Col) + (U.Row * V.Row)) / (Math.Sqrt((U.Row ^ 2) + (U.Col ^ 2)) * Math.Sqrt((V.Row ^ 2) + (V.Col ^ 2)));
            double radAngle = 0;
            if (cosAngle <= 1 && cosAngle >= -1)
            {
                radAngle = Math.Acos(cosAngle);
            }
            return (int)(radAngle * (180 / Math.PI));
        }

        /// <summary>
        /// takes the carrier as the perspective to calculate the Location of all the other guardiens 
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        public Location CalculateVectorOfFormation(Carrier carrier)
        {
            Location upperDot = carrier.Pirate.Location.Towards(carrier.Destination, carrier.Pirate.PushRange*2);
            return upperDot;
        }

        /// <summary>
        /// Calculates the best option for a capsule to go to...
        /// will always return a capsule.
        /// a good capsule is defined by:
        /// - distance from me.
        /// - holds capsule init.
        /// - no enemys guarding it (found at most 1000 units from it) //not implemented yet
        /// </summary>
        /// <returns></returns>
        public Location CalculateBEstCapsuleToGoTo(Pirate pirate)
        {
            List<Capsule> capsules = GameSettings.Game.GetMyCapsules().ToList();
            capsules = capsules.OrderBy(Capsule => Capsule.InitialLocation.Distance(pirate)).ToList();
            int RightPoints = 0;
            foreach (Capsule c in capsules)
            {
                if(c.Holder != null)
                {
                    continue;
                }
                else
                {
                    return c.Location;
                }
            }
            return capsules[0].Location;
        }
    }
}
