using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pirates;

namespace MyBot
{
    //2 lists of attackers and defenders
    public class AllPirates
    {
        private AttackerList attackerList;
        private DefenderList defenderList;

        public AllPirates()
        {

        }

        public AllPirates(PirateGame game)
        {
            this.attackerList = new AttackerList(game);
            this.defenderList = new DefenderList(game);
        }

        public AllPirates(AttackerList Alist, DefenderList Dlist)
        {
            this.attackerList = Alist;
            this.defenderList = Dlist;
        }

        public AttackerList AttackerList
        {
            get
            {
                return attackerList;
            }

            set
            {
                attackerList = value;
            }
        }

        public DefenderList DefenderList
        {
            get
            {
                return defenderList;
            }

            set
            {
                defenderList = value;
            }
        }
    }
}
