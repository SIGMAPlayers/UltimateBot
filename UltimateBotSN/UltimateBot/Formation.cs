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

        public Formation()
        {

        }

        public void SailToTarget(Location target)
        {
            if (Tail == null && Carrier != null && Bodyguard != null && Bodyguard2 != null)
            {

                FormUpAndSail3Pos(target);
                GameSettings.game.Debug("3pos");
            }
            else if (Tail != null && Carrier != null && Bodyguard != null && Bodyguard2 != null)
            {
                GameSettings.game.Debug("4pos");
                FormUpAndSail4Pos(target);

            }
        }

        public void RoleAssign(Pirate pirate)
        {
            if (Carrier == null)
                Carrier = pirate;
            else if (Bodyguard == null)
                Bodyguard = pirate;
            else if (Bodyguard2 == null)
                Bodyguard2 = pirate;
            else if (Tail == null && pirate != Carrier && pirate != Bodyguard && pirate != Bodyguard2)
                Tail = pirate;

        }

        public void ClearRoles()
        {
            if (Carrier != null)
                Carrier = null;
            if (Tail != null)
                Tail = null;
            if (Bodyguard2 != null)
                Bodyguard2 = null;
            if (Bodyguard != null)
                Bodyguard = null;
        }
        static List<Pirate> enemytargets = null;
        /// <summary>
        /// Form up in 4 pirates formation and sail the the given target.
        /// </summary>
        /// <remarks> This code was created by @Idan, for any question about wtf this code is for just ask him :)</remarks>
        /// <param name="target"> A given location the formation is wanted to head for.</param>
        public void FormUpAndSail4Pos(Location target)
        {
            Location upperDot = carrier.Location.Towards(target, carrier.PushRange);

            int x, y;
            Location U = upperDot.Subtract(carrier.GetLocation());
            y = (int)System.Math.Sqrt((90000 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
            x = ((-1) * U.Row * y) / (U.Col);
            Location V = new Location(x, y);
            List<Pirate> Threatingenemys = GeneralMethods.UnderThreat(Carrier,1000);
            if (Threatingenemys.Count == 0)
            {
                enemytargets = null;
                if (tail.Distance(upperDot.Add(U.Multiply(0.5))) <= 10 &&
                    bodyguard.Distance(upperDot.Add(V)) <= 10 &&
                    bodyguard2.Distance(upperDot.Subtract(V)) < 10)
                {
                    if (!AttackersTryPush(carrier, carrier.Location.Add(U.Multiply(-5))))
                        carrier.Sail(target);
                    if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard.Sail(target.Add(U).Add(V));
                    if (!AttackersTryPush(bodyguard2, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard2.Sail(target.Add(U).Subtract(V));
                    if (!AttackersTryPush(tail, carrier.Location.Add(U.Multiply(-5))))
                        tail.Sail(target.Add(U));
                }
                else
                {


                    if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard.Sail(upperDot.Add(V));
                    if (!AttackersTryPush(bodyguard2, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard2.Sail(upperDot.Subtract(V));
                    if (!AttackersTryPush(tail, carrier.Location.Add(U.Multiply(-5))))
                        tail.Sail(upperDot.Add(U.Multiply(0.5)));
                }
            }
            else
            {
                if(enemytargets == null)
                    enemytargets = Threatingenemys;
                if (!AttackersTryPush(carrier, carrier.Location.Add(U.Multiply(-5))))
                    carrier.Sail(target);

                foreach(Attacker attacker in GameSettings.AtkList)
                {
                    if (!attacker.Pirate.Equals(carrier))
                    {
                        enemytargets.OrderBy(Pirate => Pirate.Distance(attacker.Pirate));
                        targetedPushing(attacker.Pirate, enemytargets[0]);
                    }
                }
            }
        }

        public void targetedPushing(Pirate attacker, Pirate enemy)
        {
            if (attacker.CanPush(enemy))
                attacker.Push(enemy, new Location(0, 0));
            else
                attacker.Sail(enemy);
        }
        
        public void FormUpAndSail3Pos(Location target)
        {
            Location upperDot = carrier.Location.Towards(target, carrier.PushRange);
            int x, y;
            Location U = upperDot.Subtract(carrier.GetLocation());
            y = (int)System.Math.Sqrt((40000 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
            x = ((-1) * U.Row * y) / (U.Col);
            Location V = new Location(x, y);
            List<Pirate> Threatingenemys = GeneralMethods.UnderThreat(Carrier, 1000);
            if (Threatingenemys.Count == 0)
            {
                enemytargets = null;
                if (
                    bodyguard.Distance(upperDot.Add(V)) <= 50 &&
                    bodyguard2.Distance(upperDot.Subtract(V)) < 50)
                {
                    if (!AttackersTryPush(carrier, carrier.Location.Add(U.Multiply(-5))))
                        carrier.Sail(target);
                    if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard.Sail(target.Add(U).Add(V));
                    if (!AttackersTryPush(bodyguard2, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard2.Sail(target.Add(U).Subtract(V));
                }
                else
                {
                    if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard.Sail(upperDot.Add(V));
                    if (!AttackersTryPush(bodyguard2, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard2.Sail(upperDot.Subtract(V));

                }
            }
            else
            {
                if (enemytargets == null)
                    enemytargets = Threatingenemys;
                if (!AttackersTryPush(carrier, carrier.Location.Add(U.Multiply(-5))))
                    carrier.Sail(target);

                foreach (Attacker attacker in GameSettings.AtkList)
                {
                    if (!attacker.Pirate.Equals(carrier) && attacker != null && attacker.Pirate != null)
                    {
                        enemytargets.OrderBy(Pirate => Pirate.Distance(attacker.Pirate));
                        targetedPushing(attacker.Pirate, enemytargets[0]);
                    }
                }
            }

        }

        public static bool AttackersTryPush(Pirate pirate, Location PushLoc)
        {
            // Go over all enemies.

            foreach (Pirate enemy in GameSettings.game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (pirate.CanPush(enemy))
                {
                    //Changed
                    //Push enemy!

                    //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                    pirate.Push(enemy, PushLoc);
                    //Print a message.
                    GameSettings.game.Debug("pirate " + pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);
                    //Did push.
                    return true;
                }
            }
            // Didn't push.
            return false;
        }


    }
}
