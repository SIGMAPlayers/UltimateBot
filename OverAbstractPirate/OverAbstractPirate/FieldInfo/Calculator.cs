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
    }
}
