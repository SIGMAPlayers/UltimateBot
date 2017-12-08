using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pirates;

namespace MyBot
{
    /// <summary>
    /// This is an example for a bot.
    /// </summary>
    public class MyBot : IPirateBot
    {
        /// <summary>
        /// Makes the bot run a single turn.
        /// </summary>
        /// <param name="game">The current game state</param>

        Pirate collector;
        Pirate tailGuard;
        List<Pirate> bodyGuards;

        int offsetX;
        int offsetY;

        /// <summary>
        /// Makes the bot run a single turn.
        /// </summary>
        /// <param name="game">The current game state</param>
        // Changed Pirates to defenders where needed &
        // Changed the defenders to two layer code
        public void DoTurn(PirateGame game)
        {
            collector = game.GetAllMyPirates().ToList()[6];

            tailGuard = game.GetAllMyPirates().ToList()[7];

            bodyGuards = new List<Pirate>
            {
                game.GetAllMyPirates().ToList()[5],
                game.GetAllMyPirates().ToList()[4]
            };

            offsetX = 300;
            offsetY = 300;

            //Get one of my pirates.
            //Pirate pirate = game.GetMyLivingPirates()[0];
            List<Defender> defenders = game.GetAllMyPirates().Cast<Defender>().ToList();
            defenders.OrderBy(Pirate => Pirate.Location.Distance(game.GetEnemyCapsule().Location));
            defenders.RemoveRange(4, 4);

            foreach (Defender defender in defenders)
            {
                if (defender.IsAlive())
                {
                    if (!TryPush(defender, game))
                    {
                        // Enemy capsule defenders work
                        Location start;
                        if ((defenders[0].Equals(defender) || defenders[1].Equals(defender)))
                        {
                            defender.Layer = Roles.front;
                            if (defender.DefendFrom(game) == null)
                            {
                                start = defender.ProtectFromCarriers(0, game);
                                defender.Sail(start);
                            }
                            else
                                defender.Sail(defender.DefendFrom(game));
                        }
                        else if (defenders[2].Equals(defender) || defenders[3].Equals(defender))
                        {
                            defender.Layer = Roles.backup;
                            if (defender.DefendFrom(game) == null)
                            {
                                start = defender.ProtectFromCarriers(450, game);
                                defender.Sail(start);
                            }
                            else
                                defender.Sail(defender.DefendFrom(game));
                        }
                    }
                }
            }

            Formation(game);
        }

        /// <summary>
        /// Makes the pirate try to push an enemy pirate. Returns true if it did.
        /// </summary>
        /// <param name="pirate">The pushing pirate.</param>
        /// <param name="game">The current game state.</param>
        /// <returns> true if the pirate pushed. </returns>
        private bool TryPush(Pirate pirate, PirateGame game)
        {
            // Go over all enemies.
            foreach (Pirate enemy in game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (pirate.CanPush(enemy))
                {
                    //Changed
                    //Push enemy!
                    Location oppositeSide = enemy.GetLocation().Subtract(game.GetEnemyMothership().GetLocation());
                    //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                    oppositeSide = enemy.GetLocation().Towards(enemy.GetLocation().Add(oppositeSide), 600);
                    pirate.Push(enemy, oppositeSide);
                    //Print a message.
                    System.Console.WriteLine("pirate " + pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);
                    //Did push.
                    return true;
                }
            }
            // Didn't push.
            return false;
        }

        /// <summary>
        /// Creates the formation for the attackers
        /// </summary>
        /// <param name="game">The current game state.</param>
        /// <returns> Well... Returns nothings xd </returns>
        private void Formation(PirateGame game)
        {
            if (collector.IsAlive())
            {
                if (collector.Capsule == null)
                {
                    if (!TryPush(collector, game))
                    {
                        collector.Sail(game.GetMyCapsule());
                    }
                }
                else
                {
                    if (!TryPush(collector, game))
                    {
                        collector.Sail(game.GetMyMothership().Location);
                    }
                }
            }
            else
            {
                Pirate temp = collector;

                for (int i = 0; i < 2; i++)
                {
                    if (bodyGuards[i].IsAlive() && !collector.IsAlive())
                    {
                        collector = bodyGuards[i];
                        bodyGuards[i] = temp;
                        break;
                    }
                }
            }
            if (bodyGuards[0].IsAlive())
            {
                if (!TryPush(bodyGuards[0], game))
                    bodyGuards[0].Sail(new Location(collector.Location.Row + offsetY, collector.Location.Col + offsetX));
            }
            if (bodyGuards[1].IsAlive())
            {
                if (!TryPush(bodyGuards[1], game))
                    bodyGuards[1].Sail(new Location(collector.Location.Row + offsetY, collector.Location.Col - offsetX));
            }
            if (tailGuard.IsAlive())
            {
                if (tailGuard.PushDistance + 600 >= collector.Distance(game.GetMyMothership()) && collector.Capsule != null)
                {
                    // bodyGuards[0].Push(collector,game.GetMyMothership());
                    tailGuard.Push(collector, game.GetMyMothership());
                }
                else
                {
                    if (!TryPush(tailGuard, game))
                        tailGuard.Sail(new Location(collector.Location.Row, collector.Location.Col));
                }
            }
        }

        /// <summary>
        /// Checks if the enemy pirate is close enough to the border to kill him. 
        /// Returns the location that if you push it towards it, the pirate will die or null if you can't kill it.
        /// </summary>
        /// <param name="enemyPirate">The enemy pirate to be checked.</param>
        /// <param name="range">The range that will be checked if you can throw it</param>
        /// <param name="game">The current game state.</param>
        /// <returns>Returns the location that if you push it towards it, the pirate will die or null if you can't kill it.</returns>
        public static Location GetCloseEnoughToBorder(Pirate enemyPirate, int range, PirateGame game)
        {
            Location center = new Location(game.Rows / 2, game.Cols / 2);

            if (center.Col > enemyPirate.Location.Col && center.Row > enemyPirate.Location.Row)
            {
                int colDistance = enemyPirate.Distance(new Location(0, enemyPirate.Location.Col));
                int rowDistance = enemyPirate.Distance(new Location(enemyPirate.Location.Row, game.Cols));
                if (colDistance > rowDistance)
                    if (rowDistance <= range)
                        return new Location(0, enemyPirate.Location.Col);
                    else if (colDistance < range)
                        return new Location(enemyPirate.Location.Row, game.Cols);
                return null;
            }
            else if (center.Col > enemyPirate.Location.Col && center.Row < enemyPirate.Location.Row)
            {
                int colDistance = enemyPirate.Distance(new Location(game.Cols, enemyPirate.Location.Row));
                int rowDistance = enemyPirate.Distance(new Location(enemyPirate.Location.Col, game.Rows));
                if (colDistance > rowDistance)
                    if (rowDistance <= range)
                        return new Location(game.Cols, enemyPirate.Location.Row);
                    else if (colDistance < range)
                        return new Location(enemyPirate.Location.Col, game.Rows);
                return null;
            }
            else if (center.Col < enemyPirate.Location.Col && center.Row < enemyPirate.Location.Row)
            {
                int colDistance = enemyPirate.Distance(new Location(0, enemyPirate.Location.Col));
                int rowDistance = enemyPirate.Distance(new Location(enemyPirate.Location.Row, 0));
                if (colDistance > rowDistance)
                    if (rowDistance <= range)
                        return new Location(0, enemyPirate.Location.Col);
                    else if (colDistance < range)
                        return new Location(enemyPirate.Location.Row, 0);
                return null;
            }
            else if (center.Col < enemyPirate.Location.Col && center.Row > enemyPirate.Location.Row)
            {
                int colDistance = enemyPirate.Distance(new Location(enemyPirate.Location.Row, 0));
                int rowDistance = enemyPirate.Distance(new Location(game.Rows, enemyPirate.Location.Col));
                if (colDistance > rowDistance)
                    if (rowDistance <= range)
                        return new Location(enemyPirate.Location.Row, 0);
                    else if (colDistance < range)
                        return new Location(game.Rows, enemyPirate.Location.Col);
                return null;
            }

            return null;
        }

        /// <summary>
        /// Defends the enemy Mothership with SmartDefending
        /// </summary>
        /// <remarks>(For real explanation ask me, Booba, Idan (or Matan Meushar :) )</remarks>
        /// <param name="defenderList">List of all the defenders</param>
        /// <returns>Nothing LOL xD</returns>
        public void SmartDefending(List<Defender> defenderList)
        {

        }
    }
}