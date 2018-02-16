using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;
namespace MyBot
{
    /// <summary>
    /// responsible for the communication between every strategy
    /// + lets all strategys know all strategy's;
    /// if a strategy needs to know something about other strategy, she can do it from here.
    /// every action that needs to be done between strategy will be done here.
    /// </summary>
    public static class Communicator
    {
        
        /// <summary>
        /// Every strategy in the game that needs to communicate with other strategys.
        /// </summary>
        public static Formation formation;
        public static FireWall fireWall;

        
        /// <summary>
        /// operational mathod, two pirates will swap.
        /// </summary>
        /// <param name="a">the pirate who swaps, he will be the one to be disabled</param>
        /// <param name="b">the pirate who is being swaped, he can continue with his life normally </param>
        private static void MakeASwap(Pirate a, Pirate b)
        {
            a.SwapStates(b);
        }
        /// <summary>
        /// make sure that the defence gets all the heavys possible.
        /// </summary>
        public static void GiveHeavysToDefence()
        {
            List<BaseAttacker> heavyAttackers = new List<BaseAttacker>();
            foreach(BaseAttacker attacker in formation.Participants.Cast<BaseAttacker>().ToList())
            {
                if(attacker.Pirate.StateName == "heavy")
                {
                    heavyAttackers.Add(attacker);
                }
            }
                foreach(BaseDefender defender in fireWall.Participants.Cast<BaseDefender>().ToList())
                {
                    if(defender.Pirate.StateName == "normal")
                    {
                        if(heavyAttackers.Count > 0)
                        {
                            MakeASwap(defender.Pirate, heavyAttackers[0].Pirate);
                            heavyAttackers.RemoveAt(0);
                        }
                    }
                }
            
        }


    }
}
