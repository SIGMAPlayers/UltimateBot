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
        /// the head meneger of the formation this methods decides in which formation to choose sails accordingly
        /// </summary>
        /// <param name="target">the place that you want to go to</param>
        public void SailToTarget(Location target)
        {
            this.target = target;
           
            AssignFormationLocations();
            bool formationComlete = FormUp();
            if (formationComlete)
            {
                if (carrier != null && !AttackersTryPush(carrier))
                {
                    Carrier.Sail(target);
                }
                    
                if (Bodyguard != null && !AttackersTryPush(bodyguard))
                {
                    if(bodyguard2 == null)
                    {
                        Bodyguard.Sail(target.Add(headGuardLoc.Subtract(Carrier.Location)));
                    }
                    else
                    {
                        Bodyguard.Sail(target.Add(bodyguardLoc.Subtract(Carrier.Location)));
                    }       
                }
                if (bodyguard2 != null && !AttackersTryPush(bodyguard2))
                {
                    Bodyguard2.Sail(target.Add(bodyguard2Loc.Subtract(Carrier.Location)));
                }
                if (HeadGuard != null && !AttackersTryPush(HeadGuard))
                {
                    HeadGuard.Sail(target.Add(headGuardLoc.Subtract(Carrier.Location)));
                }
            }
                
        }
        #endregion

        #region FormUp
        /// <summary>
        /// Form up the pirates until every one of the occupied roles is in position
        /// </summary>
        /// <returns>
        /// true if the formation is complete
        /// false if the formation is incomlete.
        /// </returns>
        private bool FormUp()
        {
            int PiratesInFormation = 0;
            int PiratesInPosition = 0;
            if (HeadGuard != null)
            {
                PiratesInFormation++;
                if (HeadGuard.Distance(headGuardLoc) > 10)
                {
                    if (!AttackersTryPush(HeadGuard))
                    {
                        HeadGuard.Sail(headGuardLoc);
                    }
                }
                else
                {
                    PiratesInPosition++;
                }
                
            }
            if (Bodyguard != null)
            {
                PiratesInFormation++;
                if (Bodyguard2 != null)
                {
                    if (Bodyguard.Distance(bodyguardLoc) > 10)
                    {
                        if (!AttackersTryPush(Bodyguard))
                        {
                            bodyguard.Sail(bodyguardLoc);
                        }
                    }
                    else
                    {
                        PiratesInPosition++;
                    }
                }
                else if (Bodyguard2 == null)
                {
                    if (Bodyguard.Distance(headGuardLoc) > 10)
                    {
                        if (!AttackersTryPush(Bodyguard))
                        {
                            bodyguard.Sail(headGuardLoc);
                        }
                    }
                    else
                    {
                        PiratesInPosition++;
                    } 
                }
            }
            if (bodyguard2 != null)
            {
                PiratesInFormation++;
                if(bodyguard2.Distance(bodyguard2Loc) > 10)
                {
                    if (!AttackersTryPush(bodyguard2))
                    {
                        bodyguard2.Sail(bodyguard2Loc);
                    }
                }
                else
                {
                    PiratesInPosition++;
                }
            }

            if (PiratesInPosition == PiratesInFormation)
                return true;
            else
                return false;
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

        #region AssignFormationLocations
        /// <summary>
        ///     when called, the function evaluates the locations of the guardiens according to the formation shape using vectors
        /// </summary>
        private void AssignFormationLocations()
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
        #endregion

        #region TargetedPushing
        public void TargetedPushing(Pirate attacker, Pirate enemy)
        {
            if (attacker.CanPush(enemy))
                attacker.Push(enemy, new Location(0, 0));
            else
                attacker.Sail(Carrier.Location.Towards(enemy, attacker.PushRange));
        }
        #endregion

        #region AttackersTryPush
        private bool AttackersTryPush(Pirate pirate)
        {
            Location upperDot = carrier.Location.Towards(target, carrier.PushRange);
            Location U = upperDot.Subtract(carrier.GetLocation());

            foreach (Pirate enemy in GameSettings.game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (pirate.CanPush(enemy))
                {
                    //Changed
                    //Push enemy!

                    //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                    pirate.Push(enemy, carrier.Location.Add(U.Multiply(-5)));
                    //Print a message.
                    GameSettings.game.Debug("pirate " + pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);
                    //Did push.
                    return true;
                }
            }
            // Didn't push.
            return false;
        }
        #endregion

    }
}
