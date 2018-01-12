using System.Collections.Generic;
using System.Linq;


using Pirates;

namespace MyBot
{
    public class Formation
    {
        Pirate carrier; //holds the capsule. PROTECT HIM!
        Pirate bodyguard;
        Pirate bodyguard2;
        Pirate headGuard;
        private Location target;
        private Location bodyguardLoc;
        private Location bodyguard2Loc;
        private Location headGuardLoc;
        static List<Pirate> enemytargets = null; //when underthreat assigns the formation to prtect the carrier
        public Pirate HeadGuard { get => headGuard; set => headGuard = value; }
        public Pirate Bodyguard2 { get => bodyguard2; set => bodyguard2 = value; }
        public Pirate Bodyguard { get => bodyguard; set => bodyguard = value; }
        public Pirate Carrier { get => carrier; set => carrier = value; }
        /// <summary>
        /// Constructor to create the Formation.
        /// </summary>
        public Formation()
        {

        }

        #region SailToTarget
        /// <summary>
        /// the head meneger of the formation this methods decides in which formation to choose and sends to him 
        /// </summary>
        /// <param name="target">the place that you want to go to</param>
        public void SailToTarget(Location target)
        {
            this.target = target;
            if (HeadGuard == null && Carrier != null && Bodyguard != null && Bodyguard2 != null)
            {

                FormUpAndSail3Pos();
                GameSettings.game.Debug("3pos");
            }
            else if (HeadGuard != null && Carrier != null && Bodyguard != null && Bodyguard2 != null)
            {
                GameSettings.game.Debug("4pos");
                FormUpAndSail4Pos();

            }
            else if (HeadGuard == null && Carrier != null && Bodyguard != null && Bodyguard2 == null)
            {
                GameSettings.game.Debug("2pos");
                FormUpAndSail2Pos();

            }
        }
        #endregion

        #region RoleAssign
        /// <summary>
        /// assigns roles to the pirates who are close to the carrier.
        /// expl:the formation starts empty and gets pirates as late binding.
        /// </summary>
        /// <param name="pirate">the pirate that we want to give role to</param>
        public void RoleAssign(Pirate pirate)
        {
            if (Carrier == null)
                Carrier = pirate;
            else if (Bodyguard == null && pirate != Carrier && pirate != Bodyguard2 && pirate != HeadGuard)
                Bodyguard = pirate;
            else if (Bodyguard2 == null && pirate != Carrier && pirate != Bodyguard && pirate != HeadGuard)
                Bodyguard2 = pirate;
            else if (HeadGuard == null && pirate != Carrier && pirate != Bodyguard && pirate != Bodyguard2)
                HeadGuard = pirate;

        }
        #endregion

        #region ClearRoles
        /// <summary>
        /// the opposite from RoleAssign. clears the roles in order to break the formation.
        /// </summary>
        public void ClearRoles()
        {
            if (Carrier != null)
                Carrier = null;
            if (HeadGuard != null)
                HeadGuard = null;
            if (Bodyguard2 != null)
                Bodyguard2 = null;
            if (Bodyguard != null)
                Bodyguard = null;
        }
        #endregion


        
        private void FormationLocationsAssign(Location target)
        {
            Location upperDot = carrier.Location.Towards(target, carrier.PushRange);

            int x, y;
            Location U = upperDot.Subtract(carrier.GetLocation());
            y = (int)System.Math.Sqrt((90000 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
            x = ((-1) * U.Row * y) / (U.Col);
            Location V = new Location(x, y);

            headGuardLoc = upperDot.Add(U.Multiply(0.5));
            bodyguardLoc = upperDot.Add(V);
            bodyguard2Loc = upperDot.Subtract(V);

        }
        /// <summary>
        /// Form up in 4 pirates formation and sail the the given target.
        /// </summary>
        /// <remarks> This code was created by @Idan, for any question about wtf this code is for just ask him :)</remarks>
        /// <param name="target"> A given location the formation is wanted to head for.</param>
        public void FormUpAndSail4Pos()
        {
            Location upperDot = carrier.Location.Towards(target, carrier.PushRange);
            
            int x, y;
            Location U = upperDot.Subtract(carrier.GetLocation());
            y = (int)System.Math.Sqrt((90000 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
            x = ((-1) * U.Row * y) / (U.Col);
            Location V = new Location(x, y);
            List<Pirate> ThreatingEnemys = GeneralMethods.UnderThreat(Carrier, 1000);
            if (ThreatingEnemys.Count == 0)
            {
                enemytargets = null;
                if (HeadGuard.Distance(headGuardLoc) <= 10 &&
                    bodyguard.Distance(bodyguardLoc) <= 10 &&
                    bodyguard2.Distance(bodyguard2Loc) < 10)
                {
                    if (!AttackersTryPush(carrier, carrier.Location.Add(U.Multiply(-5))))
                        carrier.Sail(target);
                    if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard.Sail(target.Add(U).Add(V));
                    if (!AttackersTryPush(bodyguard2, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard2.Sail(target.Add(U).Subtract(V));
                    if (!AttackersTryPush(HeadGuard, carrier.Location.Add(U.Multiply(-5))))
                        HeadGuard.Sail(target.Add(U));
                }
                else
                {


                    if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard.Sail(bodyguardLoc);
                    if (!AttackersTryPush(bodyguard2, carrier.Location.Add(U.Multiply(-5))))
                        bodyguard2.Sail(bodyguard2Loc);
                    if (!AttackersTryPush(HeadGuard, carrier.Location.Add(U.Multiply(-5))))
                        HeadGuard.Sail(headGuardLoc);
                }
            }
            else
            {
                if (enemytargets == null)
                    enemytargets = ThreatingEnemys;
                if (!AttackersTryPush(carrier, carrier.Location.Add(U.Multiply(-5))))
                    carrier.Sail(target);

                //  foreach(Attacker attacker in GameSettings.AtkList)
                //   {
                //      if(!attacker.Pirate.Equals(carrier))
                //      {
                //      enemytargets.OrderBy(Pirate => Pirate.Distance(attacker.Pirate));
                //      targetedPushing(attacker.Pirate, enemytargets[0]);
                //      }
                //  }
                if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                    bodyguard.Sail(bodyguardLoc);
                if (!AttackersTryPush(bodyguard2, carrier.Location.Add(U.Multiply(-5))))
                    bodyguard2.Sail(bodyguard2Loc);
                if (!AttackersTryPush(HeadGuard, carrier.Location.Add(U.Multiply(-5))))
                    HeadGuard.Sail(target.Add(U));
            }
        }

        public void FormUpAndSail3Pos()
        {
            Location upperDot = carrier.Location.Towards(target, carrier.PushRange);
            int x, y;
            Location U = upperDot.Subtract(carrier.GetLocation());
            y = (int)System.Math.Sqrt((40000 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
            x = ((-1) * U.Row * y) / (U.Col);
            Location V = new Location(x, y);



            if (
                bodyguard.Distance(bodyguardLoc) <= 50 &&
                bodyguard2.Distance(bodyguard2Loc) < 50)
            {
                if (!AttackersTryPush(carrier, carrier.Location.Add(U.Multiply(-5))))
                    carrier.Sail(target);
                if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                    bodyguard.Sail(bodyguardLoc);
                if (!AttackersTryPush(bodyguard2, carrier.Location.Add(U.Multiply(-5))))
                    bodyguard2.Sail(bodyguard2Loc);
            }
            else
            {
                if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                    bodyguard.Sail(bodyguardLoc);
                if (!AttackersTryPush(bodyguard2, carrier.Location.Add(U.Multiply(-5))))
                    bodyguard2.Sail(bodyguard2Loc);

            }


        }

        public void FormUpAndSail2Pos()
        {
            Location upperDot = carrier.Location.Towards(target, carrier.PushRange);
            int x, y;
            Location U = upperDot.Subtract(carrier.GetLocation());
            y = (int)System.Math.Sqrt((40000 ^ 2) / ((U.Row ^ 2) / U.Col ^ 2) + 1);
            x = ((-1) * U.Row * y) / (U.Col);
            Location V = new Location(x, y);



            if (
                bodyguard.Distance(upperDot) <= 50)

            {
                if (!AttackersTryPush(carrier, carrier.Location.Add(U.Multiply(-5))))
                    carrier.Sail(target);
                if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                    bodyguard.Sail(target.Add(U));

            }
            else
            {
                if (!AttackersTryPush(bodyguard, carrier.Location.Add(U.Multiply(-5))))
                    bodyguard.Sail(upperDot);


            }


        }

        public void targetedPushing(Pirate attacker, Pirate enemy)
        {
            if (attacker.CanPush(enemy))
                attacker.Push(enemy, new Location(0, 0));
            else
                attacker.Sail(enemy);
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
