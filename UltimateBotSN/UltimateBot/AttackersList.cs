﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pirates;

namespace MyBot
{
    //A list of all attackers
    public class AttackerList : List<Attacker>
    {
        public AttackerList(PirateGame game)
        {
            PirateList all = new PirateList(game.GetAllMyPirates().OrderBy(Pirate => Pirate.Location.Distance(game.GetMyCapsule().Location)));
            for (int i = 0; i < GameSettings.FORMATION_COUNT / 4; i++)
            {
                this.Add(new Attacker(all[0]));
                this.Add(new Attacker(all[1]));
                this.Add(new Attacker(all[2]));
                this.Add(new Attacker(all[3]));
            }

        }

        public AttackerList() { }

        public AttackerList(IEnumerable<Attacker> list) : base(list) { }



    }
}
