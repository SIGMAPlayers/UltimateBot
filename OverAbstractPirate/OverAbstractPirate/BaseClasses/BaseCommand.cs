using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pirates;

namespace MyBot
{
    public abstract class BaseCommand : ICommand
    {
        public abstract void ExecuteCommand();

        public Location GetLocationSmartly()
        {
            return new Location(0, 0);
        }
    }
}
