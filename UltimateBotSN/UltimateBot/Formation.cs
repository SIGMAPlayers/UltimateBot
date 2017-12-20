using System.Collections.Generic;
using System.Linq;


using Pirates;

namespace MyBot
{
    public class Formation
    {
        Pirate carrier;
        Pirate bodyguard;
        Pirate bodyguard2;
        Pirate tail;

        public Pirate Tail { get => tail; set => tail = value; }
        public Pirate Bodyguard2 { get => bodyguard2; set => bodyguard2 = value; }
        public Pirate Bodyguard { get => bodyguard; set => bodyguard = value; }
        public Pirate Carrier { get => carrier; set => carrier = value; }
        /// <summary>
        /// Constructor to create the Formation.
        /// </summary>
        /// <param name="p1">The Carrier Pirate.</param>
        /// <param name="p2">The Bodyguard Pirate one.</param>
        /// <param name="p3">The Bodyguard Pirate two.</param>
        /// <param name="p4">The Tail Pirate.</param>
        public Formation(Pirate p1, Pirate p2, Pirate p3, Pirate p4)
        {
            this.carrier = p1;
            this.bodyguard = p2;
            this.bodyguard2 = p3;
            this.tail = p4;
        }
        public Formation()
        {

        }


        public void RoleAssign(Pirate pirate)
        {
            if (Carrier == null)
                Carrier = pirate;
            else if (Bodyguard == null)
                Bodyguard = pirate;
            else if (Bodyguard2 == null)
                Bodyguard2 = pirate;
            else if (Tail == null)
                Tail = pirate;

        }

        /// <summary>
        /// Form up in 4 pirates formation and sail the the given target.
        /// </summary>
        /// <remarks> This code was created by @Idan, for any question about wtf this code is for just ask him :)</remarks>
        /// <param name="target"> A given location the formation is wanted to head for.</param>
        public void FormUpAndSail4Pos(Location target)
        {
            Location upperDot = target.Towards(carrier, carrier.PushRange / 3);
            int x, y;
            Location U = upperDot.Subtract(carrier.GetLocation());
            y = (int)System.Math.Sqrt((200 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
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
        public void FormUpAndSail3Pos(Location target)
        {
            Location upperDot = target.Towards(carrier, carrier.PushRange / 3);
            int x, y;
            Location U = upperDot.Subtract(carrier.GetLocation());
            y = (int)System.Math.Sqrt((200 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
            x = ((-1) * U.Row * y) / (U.Col);
            Location V = new Location(x, y);
            bodyguard.Sail(upperDot.Add(V));
            bodyguard2.Sail(upperDot.Subtract(V));


            if (tail.Distance(carrier.GetLocation().Subtract(U)) <= 50 &&
                bodyguard.Distance(upperDot.Add(V)) <= 50 &&
                bodyguard2.Distance(upperDot.Subtract(V)) < 50)
                carrier.Sail(target);

        }


    }
}
