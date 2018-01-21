using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;


namespace MyBot
{
    public abstract class BaseAttacker : ICommand
    {
        private Pirate pirate;

        public Pirate Pirate { get => pirate; set => pirate = value; }

        public abstract void ExecuteCommand();
    }
}
