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
        /// Takes an object, a list of games, predicts the location of the object in the next turn
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Location PredictLocationByMovement (SpaceObject obj)
        {
            List<SpaceObject> spaceObjects = null;
            Location currentLocation = obj.Location;
            Location previousLocation = null;

            // First, check the type of obj
            if (obj is Pirate)
            {
                spaceObjects.AddRange(GameSettings.GameList[GameSettings.GameList.Count-2].GetEnemyLivingPirates().ToList());
            }
            else if (obj is Asteroid)
            {
                spaceObjects.AddRange(GameSettings.GameList[GameSettings.GameList.Count - 2].GetLivingAsteroids().ToList());
            }
            else return null;

            foreach (SpaceObject movingObj in spaceObjects) // Check on every SpaceObject on the list defined before:
            {
                if (movingObj.Id == obj.Id) // Is it the original object?
                {
                    previousLocation = movingObj.Location; // Then get it's past Location! put it in "previousLocation"
                    break;
                }
            }

            // Now, calculate it's predicted Location
            return currentLocation.Add(currentLocation.Subtract(previousLocation));

        }


        /// <summary>
        /// Takes a SpaceObject, and calculate it's final location based on the push and the current movement
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pushTo"></param>
        /// <param name="moveTo"></param>
        /// <returns></returns>
        public Location PredictLocationAfterPush (SpaceObject obj, Location pushTo, Location moveTo)
        {
            Location currentLocation = obj.Location;
            Location endPoint;
            int distance = 0;

            // Check the type of the SpaceObject
            if (obj is Pirate)
            {
                distance = GameSettings.Game.PirateMaxSpeed;
            }
            else if (obj is Asteroid)
            {
                distance = GameSettings.Game.AsteroidSpeed;
            }
            else
                GameSettings.Game.Debug("The SpaceObject in PredictLocationAfterPush does not match any implemented SpaceObjects");

            // Calculate the endPoint
            endPoint = currentLocation.Towards(moveTo, distance);
            endPoint = endPoint.Towards(pushTo, GameSettings.Game.PushDistance);

            return endPoint;
        }
    }
}
