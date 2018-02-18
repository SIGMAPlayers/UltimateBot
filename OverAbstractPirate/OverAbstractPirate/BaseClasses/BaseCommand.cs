using Pirates;
using System.Collections.Generic;
using System.Linq;


namespace MyBot
{
    public abstract class BaseCommand : ICommand
    {
        protected List<MapObject> bestWay;
        protected FieldAnalyzer FA;

        public BaseCommand()
        {
            bestWay = new List<MapObject>();
            FA = new FieldAnalyzer();
        }

        public abstract void ExecuteCommand();

        public virtual Location FindBestWay(Pirate pirate, MapObject des)
        {
            GameSettings.Game.Debug("Got to BaseCommand");
            return FA.GetBestHoles(pirate, des).Last().GetLocation();

        }

    }
}
