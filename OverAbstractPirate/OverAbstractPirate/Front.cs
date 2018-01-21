using System;
using System.Collections.Generic;
using System.Linq;



using Pirates;

namespace MyBot
{
    public class Front : BaseDefender
    {
        public override void ExecuteCommand()
        {

        }

        /// <summary>
        /// Makes the defender try to push an enemy pirate. Returns true if it did.
        /// If can be pushed out of the map, else push againts the motherboard.
        /// </summary>
        /// <returns> true if the pirate pushed. </returns>
        public override bool TryPush()
        {
            foreach (Pirate enemy in game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (pirate.CanPush(enemy) && enemy.HasCapsule())
                {
                    //Changed
                    //Push enemy!
                    int range = CheckHowManyCanPush(enemy);
                    Location outOfBorder = GeneralMethods.GetCloseEnoughToBorder(enemy, range);
                    if (outOfBorder != null)
                    {
                        pirate.Push(enemy, outOfBorder);
                        return true;
                    }
                    else
                    {
                        Location oppositeSide = enemy.GetLocation().Subtract(game.GetEnemyMothership().GetLocation());
                        //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                        oppositeSide = enemy.GetLocation().Towards(enemy.GetLocation().Add(oppositeSide), 600);
                        pirate.Push(enemy, oppositeSide);
                        //Print a message.
                        game.Debug("defender " + defender + " pushes " + enemy + " towards " + enemy.InitialLocation);
                        //Did push.
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
