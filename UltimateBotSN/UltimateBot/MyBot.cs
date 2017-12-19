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
        public static PirateGame game;
        Pirate collector;
        Pirate tailGuard;
        List<Pirate> bodyGuards;

        /// <summary>
        /// Makes the bot run a single turn.
        /// </summary>
        /// <param name="game">The current game state</param>
        // Changed Pirates to defenders where needed &
        // Changed the defenders to two layer code
        //a function to start the game
        public AllPirates OnGameStart(PirateGame game)
        {
            AllPirates allPirates = new AllPirates(game);
            return allPirates;
        }

        public void DoTurn(PirateGame game)
        {
            GameSettings.game = game;
            if (!GameSettings.START)
            {
                GameSettings.allPirates = OnGameStart(game);
                GameSettings.defList = GameSettings.allPirates.Dlist1;
                GameSettings.AtkList = GameSettings.allPirates.Alist1;
                GameSettings.START = true;
            }
            MyBot.game = game;
            collector = game.GetAllMyPirates().ToList()[6];

            tailGuard = game.GetAllMyPirates().ToList()[7];

            bodyGuards = new List<Pirate>
            {
                game.GetAllMyPirates().ToList()[5],
                game.GetAllMyPirates().ToList()[4]
            };

            //Get one of my pirates.
            //Pirate pirate = game.GetMyLivingPirates()[0];
            List<Defender> defenders = GameSettings.allPirates.Dlist1;
            defenders.OrderBy(Pirate => Pirate.Pirate.Location.Distance(game.GetEnemyCapsule().Location));
            defenders.RemoveRange(4, 4);

            foreach (Defender defender in defenders)
            {
                if (defender.IsAlive())
                {
                    if (!GeneralMethods.TryPush(defender.Pirate))
                    {
                        // Enemy capsule defenders work
                        Location start;
                        if ((defenders[0].Equals(defender) || defenders[1].Equals(defender)))
                        {
                            defender.Layer = Roles.front;
                            if (defender.DefendFrom(game) == null)
                            {
                                start = defender.ProtectFromCarriers(0, game);
                                defender.Pirate.Sail(start);
                            }
                            else
                                defender.Pirate.Sail(defender.DefendFrom(game));
                        }
                        else if (defenders[2].Equals(defender) || defenders[3].Equals(defender))
                        {
                            defender.Layer = Roles.backup;
                            if (defender.DefendFrom(game) == null)
                            {
                                start = defender.ProtectFromCarriers(450, game);
                                defender.Pirate.Sail(start);
                            }
                            else
                                defender.Pirate.Sail(defender.DefendFrom(game));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the enemy pirate is close enough to the border to kill him. 
        /// Returns the location that if you push it towards it, the pirate will die or null if you can't kill it.
        /// </summary>
        /// <param name="enemyPirate">The enemy pirate to be checked.</param>
        /// <param name="range">The range that will be checked if you can throw it</param>
        /// <returns>Returns the location that if you push it towards it, the pirate will die or null if you can't kill it.</returns>
        public static Location GetCloseEnoughToBorder(Pirate enemyPirate, int range)
        {
            Location up = new Location(0, enemyPirate.Location.Col);
            Location right = new Location(enemyPirate.Location.Row, MyBot.game.Cols);
            Location left = new Location(enemyPirate.Location.Row, 0);
            Location down = new Location(MyBot.game.Rows, enemyPirate.Location.Col);
            int upDistance = enemyPirate.Distance(up);
            int rightDistance = enemyPirate.Distance(right);
            int leftDistance = enemyPirate.Distance(left);
            int downDistance = enemyPirate.Distance(down);

            if (upDistance < rightDistance && upDistance < leftDistance && upDistance < downDistance)
                if(upDistance < range)
                    return up;
            else if (rightDistance < upDistance && rightDistance < leftDistance && rightDistance < downDistance)
                if(rightDistance < range)
                    return right;
            else if (leftDistance < upDistance && leftDistance < rightDistance && leftDistance < downDistance)
                if(leftDistance < range)
                    return down;
            else if(downDistance < upDistance && downDistance < rightDistance && downDistance < leftDistance)
                if(downDistance < range)
                    return left;

            //Returns null if not close enough to a border
            return null;

        //    Location center = new Location(game.Rows / 2, game.Cols / 2);

        //    if (center.Col > enemyPirate.Location.Col && center.Row > enemyPirate.Location.Row)
        //    {
        //        int colDistance = enemyPirate.Distance(new Location(0, enemyPirate.Location.Col));
        //        int rowDistance = enemyPirate.Distance(new Location(enemyPirate.Location.Row, game.Cols));
        //        if (colDistance > rowDistance)
        //            if (rowDistance <= range)
        //                return new Location(0, enemyPirate.Location.Col);
        //            else if (colDistance < range)
        //                return new Location(enemyPirate.Location.Row, game.Cols);
        //        return null;
        //    }
        //    else if (center.Col > enemyPirate.Location.Col && center.Row < enemyPirate.Location.Row)
        //    {
        //        int colDistance = enemyPirate.Distance(new Location(game.Cols, enemyPirate.Location.Row));
        //        int rowDistance = enemyPirate.Distance(new Location(enemyPirate.Location.Col, game.Rows));
        //        if (colDistance > rowDistance)
        //            if (rowDistance <= range)
        //                return new Location(game.Cols, enemyPirate.Location.Row);
        //            else if (colDistance < range)
        //                return new Location(enemyPirate.Location.Col, game.Rows);
        //        return null;
        //    }
        //    else if (center.Col < enemyPirate.Location.Col && center.Row < enemyPirate.Location.Row)
        //    {
        //        int colDistance = enemyPirate.Distance(new Location(0, enemyPirate.Location.Col));
        //        int rowDistance = enemyPirate.Distance(new Location(enemyPirate.Location.Row, 0));
        //        if (colDistance > rowDistance)
        //            if (rowDistance <= range)
        //                return new Location(0, enemyPirate.Location.Col);
        //            else if (colDistance < range)
        //                return new Location(enemyPirate.Location.Row, 0);
        //        return null;
        //    }
        //    else if (center.Col < enemyPirate.Location.Col && center.Row > enemyPirate.Location.Row)
        //    {
        //        int colDistance = enemyPirate.Distance(new Location(enemyPirate.Location.Row, 0));
        //        int rowDistance = enemyPirate.Distance(new Location(game.Rows, enemyPirate.Location.Col));
        //        if (colDistance > rowDistance)
        //            if (rowDistance <= range)
        //                return new Location(enemyPirate.Location.Row, 0);
        //            else if (colDistance < range)
        //                return new Location(game.Rows, enemyPirate.Location.Col);
        //        return null;
        //    }

        //    return null;
        }
    }
}