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
            this.AttackerList = new AttackerList(game);
            this.DefenderList = new DefenderList(game);
        }

        public AllPirates(AttackerList Alist, DefenderList Dlist)
        {
            this.AttackerList = Alist;
            this.DefenderList = Dlist;
        }

        public AttackerList AttackerList { get => attackerList; set => attackerList = value; }
        public DefenderList DefenderList { get => defenderList; set => defenderList = value; }
    }
}
