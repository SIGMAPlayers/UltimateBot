
using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    public class AsteroidHandler
    {
        FieldAnalyzer fieldAnalyzer = new FieldAnalyzer();
        int asteroidSize = GameSettings.Game.AsteroidSize;

        /// <summary>
        /// Check the most crowded place to push the astroid to
        /// </summary>
        /// <returns></returns>
        public Location FindBestLocationToPushTo()
        {
            if (GameSettings.Game.GetEnemyLivingPirates().Length == 0) // If there isn't any living enemy pirate
            {
                if (GameSettings.Game.GetEnemyMotherships().Length > 0) // If there is an enemy motherShip
                {
                    Mothership mothership = fieldAnalyzer.GetMostPopulatedEnemyMothership();
                    if (mothership != null)
                        return mothership.GetLocation(); // Push to enemy mothership
                }
                else return new Location (0,0); // For no errors purpose...
            }
                
            Location bestLocation = null;
            int maxEnemies = 0;
            int current = 0;

            List<GameObject> enemies = new List<GameObject>();
            enemies.AddRange(GameSettings.Game.GetEnemyLivingPirates());

            // Check on each enemy, which is the most crowded location
            foreach (Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
            {
                current = fieldAnalyzer.CheckHowManyGameObjectsNearAreaByDistance(enemies, enemy, asteroidSize);
                if (current > maxEnemies)
                {
                    maxEnemies = current;
                    bestLocation = enemy.Location;
                }
            }

            return bestLocation;
        }
    }
}
