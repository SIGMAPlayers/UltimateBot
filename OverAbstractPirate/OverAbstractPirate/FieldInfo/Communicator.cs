using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class Communicator
    {
        private static Communicator instance;
        private Communicator(){}
        public static Communicator GetInstance()
        {
                if (instance == null)
                {
                    instance = new Communicator();
                }
                return instance;
        }
        
        /// <summary>
        /// Every strategy in the game that needs to communicate with other strategys.
        /// </summary>
        public List<Strategy> StratList;


        /// <summary>
        /// operational mathod, two pirates will swap.
        /// </summary>
        /// <param name="a">the pirate who swaps, he will be the one to be disabled</param>
        /// <param name="b">the pirate who is being swaped, he can continue with his life normally </param>
        private void MakeASwap(Pirate a, Pirate b)
        {
            a.SwapStates(b);
        }
        /// <summary>
        /// make sure that the defence gets all the heavys possible.
        /// </summary>
        public bool GiveHeavysToDefence()
        {//                 if(Participants.OfType<Carrier>().ToList() != null)
            List<BaseAttacker> heavyAttackers = new List<BaseAttacker>();
            if(StratList.OfType<Formation>().ToList().Count > 0)
            {
                 foreach(BaseAttacker attacker in StratList.OfType<Formation>().ToList()[0].Participants.Cast<BaseAttacker>().ToList())
                {
                    if(attacker.Pirate.StateName == "heavy")
                    {
                        heavyAttackers.Add(attacker);
                    }
                    GameSettings.Game.Debug("HEVY'S AREEEE " + attacker.Pirate.StateName);
                }
            }
           
            if(StratList.OfType<FireWall>().ToList().Count > 0)
            {
                foreach(BaseDefender defender in StratList.OfType<FireWall>().ToList()[0].Participants.Cast<BaseDefender>().ToList())
                {
                    if(defender.Pirate.StateName == "normal")
                    {
                        if(heavyAttackers.Count > 0)
                        {
                            MakeASwap(defender.Pirate, heavyAttackers[0].Pirate);
                            heavyAttackers.RemoveAt(0);
                            return true;
                        }
                    }
                } 
            }
               
            return false;
        }


    }
}
