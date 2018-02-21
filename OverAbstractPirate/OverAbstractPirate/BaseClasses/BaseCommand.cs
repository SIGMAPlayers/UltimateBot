using Pirates;
using System.Collections.Generic;
using System.Linq;


namespace MyBot
{
    public abstract class BaseCommand : ICommand
    {
        protected List<MapObject> bestWay;
        protected FieldAnalyzer FA;
        protected AsteroidHandler asteroidHandler = new AsteroidHandler();

        public BaseCommand()
        {
            bestWay = new List<MapObject>();
            FA = new FieldAnalyzer();
        }

        public abstract void ExecuteCommand();

        public virtual Location FindBestWay(Pirate pirate, MapObject des)
        {
            GameSettings.Game.Debug("Got to BaseCommand");
            Location location;
            if (GameSettings.Game.Turn>2)
            {
                foreach (Asteroid asteroid in GameSettings.Game.GetLivingAsteroids())
                {
                    location = asteroidHandler.AvoidAsteroid(pirate, asteroid);
                    if (location != null)
                    {
                        return location;
                    }
                }
            }
            return FA.GetBestHoles(pirate, des).Last().GetLocation();

        }

    }
}
