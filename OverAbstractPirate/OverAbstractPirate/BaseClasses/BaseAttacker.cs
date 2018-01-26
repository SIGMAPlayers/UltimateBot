using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;


namespace MyBot
{
    public abstract class BaseAttacker : ICommand
    {
        private Pirate pirate;
        private Pirate targetEnemy;
        private Location destination;

        public Pirate Pirate { get => pirate; set => pirate = value; }
        public Pirate TargetEnemy { get => targetEnemy; set => targetEnemy = value; }
        public Location Destination { get => destination; set => destination = value; }

        public abstract void ExecuteCommand();


        #region AttackersTryPush
        protected bool AttackersTryPush(Pirate pirate)
        {
            Location upperDot = pirate.Location.Towards(Destination, pirate.PushRange);
            Location U = upperDot.Subtract(pirate.GetLocation());

            foreach (Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (pirate.CanPush(enemy))
                {
                   
                    pirate.Push(enemy, pirate.Location.Add(U.Multiply(-5)));
                    
                    GameSettings.Game.Debug("pirate " + pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);
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
