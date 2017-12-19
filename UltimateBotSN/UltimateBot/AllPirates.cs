using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pirates;

namespace MyBot
{
    //2 lists of attackers and defenders
    public class AllPirates
    {
        private AttackerList Alist;
        private DefenderList Dlist;

        public AllPirates()
        {

        }

        public AllPirates(PirateGame game)
        {
            this.Alist1 = new AttackerList(game);
            this.Dlist = new DefenderList(game);
        }

        public AllPirates(AttackerList Alist, DefenderList Dlist)
        {
            this.Alist1 = Alist;
            this.Dlist = Dlist;
        }

        public AttackerList Alist1
        {
            get
            {
                return Alist;
            }

            set
            {
                Alist = value;
            }
        }

        public DefenderList Dlist1
        {
            get
            {
                return Dlist;
            }

            set
            {
                Dlist = value;
            }
        }
    }
}
