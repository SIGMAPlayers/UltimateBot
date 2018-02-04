using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pirates;

namespace MyBot
{
    class AsteroidHandler
    {
        private FieldAnalyzer fieldAnalyzer = new FieldAnalyzer();
        private int asteroidSize = GameSettings.Game.AsteroidSize;
        private int turnsToPassAsteroid = GameSettings.Game.AsteroidSize / GameSettings.Game.PirateMaxSpeed;

        /// <summary>
        /// Check the most crowded place to push the astroid to
        /// </summary>
        /// <returns></returns>
        public Location FindBestLocationToPushTo
        {
            get
            {
                if (GameSettings.Game.GetEnemyLivingPirates()[0] == null) // If there isn't any living enemy pirate
                {
                    if (GameSettings.Game.GetEnemyMotherships()[0] != null) // If there is an enemy motherShip
                    {
                        return GameSettings.Game.GetEnemyMotherships()[0].Location; // Push to enemy mothership
                    }
                    else return new Location(0, 0); // For no errors purpose...
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

        public Location AvoidAstroid(Asteroid asteroid)
        {
            int turnsForDoomedPirates = 0;
            return null;
        }
    }
}
