using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pirates;

namespace MyBot
{
    //A list of all defenders
    public class DefenderList : List<Defender>
    {
        public DefenderList(PirateGame game)
        {
            PirateList def = new PirateList(game.GetAllMyPirates().OrderBy(Pirate => Pirate.Location.Distance(game.GetMyCapsule().Location)));
            def.RemoveRange(0, 4);
            for (int i = 0; i < (def.Count / 2); i++)
            {
                this.Add(new Defender(def[i], Roles.front));
            }
            for (int i = 0; i < def.Count - (def.Count / 2); i++)
            {
                this.Add(new Defender(def[i], Roles.backup));
            }


        }
        public DefenderList() { }

        public DefenderList(IEnumerable<Defender> list) : base(list) { }
    }
}
