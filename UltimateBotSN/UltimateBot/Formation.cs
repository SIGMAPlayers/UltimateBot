﻿using System.Collections.Generic;
using System.Linq;
using static System.Math;

using Pirates;

namespace MyBot
{
    public class Formation
    {

        Pirate carrier;
        Pirate bodyguard;
        Pirate bodyguard2;
        Pirate tail;
        public Formation(Pirate p1, Pirate p2, Pirate p3, Pirate p4)
        {
            this.carrier = p1;
            this.bodyguard = p2;
            this.bodyguard2 = p3;
            this.tail = p4;
        }
        /// <summary>
        /// form up in 4 pirates formation and sail the the given target
        /// </summary>
        /// <param name="target"> a given location the formation is wanted to head for</param>
        public void FormUpAndSail(Location target)
        {
            Location upperDot = target.Towards(carrier, carrier.PushRange / 3);
            int x, y;
            Location U = upperDot.Subtract(carrier.GetLocation());
            y = (int)Sqrt((200 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
            x = ((-1) * U.Row * y) / (U.Col);
            Location V = new Location(x, y);
            bodyguard.Sail(upperDot.Add(V));
            bodyguard2.Sail(upperDot.Subtract(V));
            tail.Sail(carrier.GetLocation().Subtract(U));

            if (tail.Distance(carrier.GetLocation().Subtract(U)) <= 50 &&
                bodyguard.Distance(upperDot.Add(V)) <= 50 &&
                bodyguard2.Distance(upperDot.Subtract(V)) < 50)
                carrier.Sail(target);
        }
    }
}