
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

        public Location FindBestLocationToPushTo(Pirate Push1)
        {
            // GameSettings.Game.Debug("FindBestLocationToPushTo");
            if (GameSettings.Game.GetLivingAsteroids().Length > 0)
            {
                Asteroid asteroidClosestToPirate = GameSettings.Game.GetLivingAsteroids().OrderBy(Asteroid => Asteroid.Distance(Push1)).ToList()[0];
                List<Asteroid> astroidsByDistanceFromAsteroid = GameSettings.Game.GetLivingAsteroids().ToList().OrderBy(Asteroid => Asteroid.Distance(asteroidClosestToPirate)).ToList();
                List<Asteroid> astroidsByDistanceFromPirate = GameSettings.Game.GetLivingAsteroids().ToList().OrderBy(Asteroid => Asteroid.Distance(Push1)).ToList();
                Location farFromPirate = asteroidClosestToPirate.Location.Subtract(Push1.Location);


                if (GameSettings.Game.GetEnemyLivingPirates().Length == 0) // If there isn't any living enemy pirate
                {
                    if (GameSettings.Game.GetEnemyCapsules().Length > 0) // If there is an enemy motherShip
                    {
                        //Mothership mothership = fieldAnalyzer.GetMostPopulatedEnemyMothership();
                        Capsule cap = GameSettings.Game.GetEnemyCapsules()[0];
                        if (cap != null && WillISurviveIfAstroidGetsPushedThere(Push1, asteroidClosestToPirate, cap.GetLocation()))
                        {
                            return cap.GetLocation(); // Push to enemy mothership
                        }
                        else if (WillAsteroidHitMe(Push1, asteroidClosestToPirate))
                        {
                            return GameSettings.LastGameLivingAsteroids[asteroidClosestToPirate.Id];
                        }
                    }
                    else
                    {
                        //if (WillAsteroidHitMe(Push1,asteroidClosestToPirate))
                        {
                            // return farFromPirate; // For no errors purpose...
                        }
                    }
                }
                else if (GameSettings.Game.GetLivingAsteroids().Length > 2)
                {
                    Asteroid secondClosestAsteroidToPirate = GameSettings.Game.GetLivingAsteroids().OrderBy(Asteroid => Asteroid.Distance(Push1)).ToList()[1];
                    // GameSettings.Game.Debug("asteroid will hithugkjknjjnj?");
                    if (!WillAsteroidHitMe(Push1, asteroidClosestToPirate) && !WillAsteroidHitMe(Push1, secondClosestAsteroidToPirate))
                    {
                        GameSettings.Game.Debug("***********************************************");
                        // GameSettings.Game.Debug("asteroid will hit? *** " + WillAsteroidHitMe(Push1, asteroidClosestToPirate));
                        Location next = this.fieldAnalyzer.PredictLocation(asteroidClosestToPirate, 4);
                        return null;
                    }
                    if (WillAsteroidHitMe(Push1, secondClosestAsteroidToPirate))
                    {
                        asteroidClosestToPirate = secondClosestAsteroidToPirate;

                    }
                    astroidsByDistanceFromPirate.RemoveAt(0);
                    if (astroidsByDistanceFromAsteroid.Count > 2 && asteroidClosestToPirate.Distance(astroidsByDistanceFromAsteroid[1]) == asteroidClosestToPirate.Distance(astroidsByDistanceFromAsteroid[2]))
                    {
                        return astroidsByDistanceFromAsteroid[2].Location;
                    }
                    foreach (Asteroid asteroid in astroidsByDistanceFromPirate)
                    {
                        //Asteroid asteroidClosestToAstroid = GameSettings.Game.GetLivingAsteroids().ToList().OrderBy(Asteroid => Asteroid.Distance(Push1)).ToList()[1];

                        if (WillISurviveIfAstroidGetsPushedThere(Push1, asteroidClosestToPirate, asteroid.Location) && WillAsteroidHitMe(Push1, asteroidClosestToPirate))
                        {
                            return asteroid.Location;
                        }
                    }

                    // GameSettings.Game.Debug("wouldn't survive asteroid on asteroid attack. check if pirate pushes asteroid on enemies");
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
            return null;
        }

        public bool WillISurviveIfAstroidGetsPushedThere(Pirate pusher, Asteroid asteroid, Location there)
        {
            Location AstroidLocationAtEndOfPush = asteroid.Location.Towards(there, pusher.PushRange);
            GameSettings.Game.Debug("distance of pusher from asteroid ==> " + pusher.Distance(AstroidLocationAtEndOfPush) + "   Asteroid size ==> " + asteroid.Size);
            if (pusher.Distance(AstroidLocationAtEndOfPush) <= asteroid.Size)
            {
                return false;
            }
            return true;
        }

        public bool WillAsteroidHitMe(Pirate pirate, Asteroid asteroid)
        {
            Location nextPirate = this.fieldAnalyzer.PredictLocation(pirate, 1);  // = pirate.Location.Add(pirateMov);
            Location nextAsteroid = this.fieldAnalyzer.PredictLocation(asteroid, 1);   // = asteroid.Location.Add(asteroidMov);

            // GameSettings.Game.Debug("WillAsteroidHitMe - next turn distance:"+nextAsteroid.Distance(nextPirate) + "...asteroid Size:" + asteroid.Size + "...nextAsteroid" + nextAsteroid + ".asteroidLoc"+ asteroid.Location + "...nextPirate" + nextPirate);
            foreach (Mothership msh in GameSettings.Game.GetMyMotherships())
            {
                if (pirate.Location.Distance(msh.Location) <= 200)
                {
                    return true;
                }
            }
            if (nextPirate != null && nextAsteroid != null && nextAsteroid.Distance(nextPirate) <= asteroid.Size)
            {
                return true;
            }

            return false;



            /*Location pirateMov=null; Location asteroidMov=null; Location nextPirate=null; Location nextAsteroid=null; Location lastPirate=null; Location lastAsteroid=null;
            List<PirateGame> pgl = GameSettings.GameList;

            //GameSettings.Game.Debug(GameSettings.GameList.Count + " " + (GameSettings.GameList.Count-2));
            //foreach(Pirate p in (GameSettings.GameList[GameSettings.GameList.Count - 2]).GetMyLivingPirates())
            //{
            //    if (pirate.Id==p.Id)

            //}
            if (pgl[pgl.Count-2].GetMyLivingPirates().Length > 0 && pgl[pgl.Count-2].GetLivingAsteroids().Length > 0)
            {
                lastPirate = (pgl[pgl.Count-2]).GetMyLivingPirates()[0].Location;
                lastAsteroid = (pgl[pgl.Count-2]).GetLivingAsteroids()[0].Location;

                pirateMov = pirate.Location.Subtract(lastPirate);
                asteroidMov = asteroid.Location.Subtract(lastAsteroid);
            }
            else
            {
                pirateMov=new Location (0,0);
                asteroidMov=new Location (0,0);
            }

            GameSettings.Game.Debug("asteroid id "+asteroid.Id+"asteroid.Location"+asteroid.Location+"...asteroidMov"+asteroidMov+"...pirateMov"+pirateMov+"...lastAsteroid"+lastAsteroid+"...lastPirate"+lastPirate);
            */


        }
    }
}
