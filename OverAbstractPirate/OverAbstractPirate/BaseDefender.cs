﻿using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;


using Pirates;

namespace MyBot
{
    public abstract class BaseDefender : ICommand
    {
        protected Pirate pirate;

        public Pirate Pirate { get => pirate; set => pirate = value; }

        /// <summary>
        /// The instruction book that the bot follows, a very stupid method that just executes a strategy that the
        /// class was ment to do
        /// </summary>
        public abstract void ExecuteCommand();

        /// <summary>
        /// Makes the defender try to push an enemy pirate. Returns true if it did.
        /// If can be pushed out of the map, else push againts the motherboard.
        /// </summary>
        /// <returns> true if the pirate pushed. </returns>
        public abstract bool TryPush();

        /// <summary>
        /// Chooses a pirate in range of their city to attack.
        /// </summary>
        /// <returns> Returns the closest pirate to attack. </returns>
        public abstract Pirate DefendFrom();

        /// <summary>
        /// Generates dynamically a guard location so the defenders will always be infront
        /// of the enemyCarrier. Returns the location the defenders need to be in.
        /// </summary>
        /// <remarks>For explanation ask me, Idan, Booba or Matan Meushar</remarks>
        /// <example for param name="range">The range from the first layer range = 0 (first) range = ~450 (second).</param>
        /// <returns> Returns the basic location for the defenders. </returns>
        /// Changed to work with two layers
        public abstract Location ProtectFromCarriers();

        public bool IsAlive()
        {
            return this.Pirate.IsAlive();
        }


    }
}
