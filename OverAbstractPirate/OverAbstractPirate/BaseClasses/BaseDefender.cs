using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    public abstract class BaseDefender : ICommand
    {
        protected Pirate pirate;
        private FieldAnalyzer fieldAnalyzer;
        private Pirate pirateToPush;
        private Location whereToPush;
        protected AsteroidHandler asteroidHandler;
        private Location whereToDefend;

        public Pirate Pirate { get => pirate; set => pirate = value; }
        public FieldAnalyzer FieldAnalyzer { get => fieldAnalyzer; set => fieldAnalyzer = value; }
        public Pirate PirateToPush { get => pirateToPush; set => pirateToPush = value; }
        public Location WhereToPush { get => whereToPush; set => whereToPush = value; }
        public AsteroidHandler AsteroidHandler { get => AsteroidHandler; set => AsteroidHandler = value; }
        public Location WhereToDefend { get => whereToDefend; set => whereToDefend = value; }

        protected BaseDefender(Pirate pirate, FieldAnalyzer fieldAnalyzer)
        {
            this.pirate = pirate;
            this.fieldAnalyzer = fieldAnalyzer;
            this.pirateToPush = null;
            this.whereToPush = null;
            this.whereToDefend = null;
            asteroidHandler = new AsteroidHandler();
        }

        /// <summary>
        /// The instruction book that the bot follows, a very stupid method that just executes a strategy that the
        /// class was meant to do
        /// </summary>
        public abstract void ExecuteCommand();

        /// <summary>
        /// Makes the defender try to push an enemy pirate. Returns true if it did.
        /// If can be pushed out of the map, else push againts the motherboard.
        /// </summary>
        /// <returns> true if the pirate pushed. </returns>
        public abstract bool Push();

        /// <summary>
        /// Chooses a pirate in range of their city to attack.
        /// </summary>
        /// <returns> Returns the closest pirate to attack. </returns>
        public abstract Pirate Protect();

        /// <summary>
        /// Generates dynamically a guard location so the defenders will always be infront
        /// of the enemyCarrier. Returns the location the defenders need to be in.
        /// </summary>
        /// <remarks>For explanation ask me, Idan, Booba or Matan Meushar</remarks>
        /// <example for param name="range">The range from the first layer range = 0 (first) range = ~450 (second).</param>
        /// <returns> Returns the basic location for the defenders. </returns>
        /// Changed to work with two layers
        public abstract Location DefendAt();

        public bool IsAlive()
        {
            return this.Pirate.IsAlive();
        }


    }
}
