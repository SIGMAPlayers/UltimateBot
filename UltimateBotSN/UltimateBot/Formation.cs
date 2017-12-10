using System.Collections.Generic;
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

        public Pirate Carrier { get => carrier; set => carrier = value; }
        public Pirate Bodyguard { get => bodyguard; set => bodyguard = value; }
        public Pirate Bodyguard2 { get => bodyguard2; set => bodyguard2 = value; }
        public Pirate Tail { get => tail; set => tail = value; }

        /// <summary>
        /// Constructor to create the Formation. (call at the beggining, than call the setters to dynamically change roles)
        /// </summary>
        /// <param name="p1">The Carrier Pirate.</param>
        /// <param name="p2">The Bodyguard Pirate one.</param>
        /// <param name="p3">The Bodyguard Pirate two.</param>
        /// <param name="p4">The Tail Pirate.</param>
        public Formation(Pirate p1, Pirate p2, Pirate p3, Pirate p4)
        {
            this.Carrier = p1;
            this.Bodyguard = p2;
            this.Bodyguard2 = p3;
            this.Tail = p4;
        }

        /// <summary>
        /// Form up in 4 pirates formation and sail the the given target.
        /// </summary>
        /// <remarks> This code was created by @Idan, for any question about wtf this code is for just ask him :)</remarks>
        /// <param name="target"> A given location the formation is wanted to head for.</param>
        public void FormUpAndSail4Pos(Location target)
        {
            Location upperDot = target.Towards(Carrier, Carrier.PushRange / 3);
            int x, y;
            Location U = upperDot.Subtract(Carrier.GetLocation());
            y = (int)Sqrt((200 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
            x = ((-1) * U.Row * y) / (U.Col);
            Location V = new Location(x, y);

            if (Tail.Distance(Carrier.GetLocation().Subtract(U)) <= 50 &&
                Bodyguard.Distance(upperDot.Add(V)) <= 50 &&
                Bodyguard2.Distance(upperDot.Subtract(V)) < 50)
                Carrier.Sail(target);

            Bodyguard.Sail(upperDot.Add(V));
            Bodyguard2.Sail(upperDot.Subtract(V));
            Tail.Sail(Carrier.GetLocation().Subtract(U));
        }

        /// <summary>
        /// Form up in 3 pirates formation and sail the the given target.
        /// in this formation the pirates will defend the carrier until he comes close enough to the target for them to sling him to there
        /// </summary>
        /// <param name="target"></param>
        public void FormUpAndSail3Pos(Location target)
        {
            if (Carrier.Distance(target) > bodyguard.PushDistance + Bodyguard2.PushDistance + Carrier.MaxSpeed)
            {
                Location upperDot = target.Towards(Carrier, Carrier.PushRange / 3);
                int x, y;
                Location U = upperDot.Subtract(Carrier.GetLocation());
                y = (int)Sqrt((200 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
                x = ((-1) * U.Row * y) / (U.Col);
                Location V = new Location(x, y);
               
                if (
                    Bodyguard.Distance(upperDot.Add(V)) <= 50 &&
                    Bodyguard2.Distance(upperDot.Subtract(V)) < 50)
                    Carrier.Sail(target);
                Bodyguard.Sail(upperDot.Add(V));
                Bodyguard2.Sail(upperDot.Subtract(V));
            }
            else
            {
                Location upperDot = target.Towards(carrier, Carrier.PushRange / 3);
                int x, y;
                Location U = (upperDot.Subtract(Carrier.GetLocation())).Multiply(-1);
                y = (int)Sqrt((200 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
                x = ((-1) * U.Row * y) / (U.Col);
                Location V = new Location(x, y);
                


                if (
                    Bodyguard.Distance(upperDot.Add(V)) <= 50 &&
                    Bodyguard2.Distance(upperDot.Subtract(V)) < 50)
                {
                    Carrier.Sail(target);
                    Bodyguard.Sail(upperDot.Add(V));
                    Bodyguard2.Sail(upperDot.Subtract(V));
                   
                }
                else
                {
                    Carrier.Sail(target);
                    Bodyguard.Push(Carrier, target);
                    Bodyguard2.Push(Carrier, target);

                }
                    
            }
        }

        /// <summary>
        /// Form up in 2 pirates formation and sail the the given target.
        /// this formation is the last resort. the Bodyguard will follow the Carrier to the point where he is under a threat.
        /// when this happens: he slings away the carrier to the target.
        /// </summary>
        /// <param name="target"></param>
        public void FormUpAndSail2Pos(Location target)
        {
           if (Carrier.InRange(target, (bodyguard.PushDistance + Carrier.MaxSpeed)))
           {
                if (GeneralMethods.UnderThreat(Carrier, 600).Equals(null))
                {
                    Bodyguard.Sail(Carrier);
                }
                else
                {
                    Bodyguard.Push(Carrier, target);
                }
           }
           else
           {
                Bodyguard.Push(Carrier, target);
           }
        }

       

    }
}
