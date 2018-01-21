using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pirates;

namespace OverAbstractPirate
{
    public abstract class BaseDefender : ICommand
    {
        protected Pirate pirate;

        public Pirate Pirate { get => pirate; set => pirate = value; }

        public abstract void ExecuteCommand();

        public abstract bool TryPush();

        public bool IsAlive()
        {
            return this.Pirate.IsAlive();
        }


    }
}
