using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pirates;

namespace MyBot
{
    //A list of pirates 
    public class PirateList : List<Pirate>
    {
        public PirateList() { }

        public PirateList(IEnumerable<Pirate> list) : base(list) { }
    }
}
