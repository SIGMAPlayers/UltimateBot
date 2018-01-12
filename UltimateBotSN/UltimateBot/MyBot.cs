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


        /// <summary>
        /// yaya
        /// </summary>
        /// <param name="game"></param>
        public void DoTurn(PirateGame game)
        {
            GameSettings.game = game;
            Formation form = null;
            if (!GameSettings.START)
            {
                GameSettings.allPirates = OnGameStart(game);
                GameSettings.defList = GameSettings.allPirates.Dlist1;
                GameSettings.AtkList = GameSettings.allPirates.Alist1;
                form = new Formation();

                GameSettings.START = true;
                form = new Formation();
            }
            MyBot.game = game;

            List<Defender> defenders = GameSettings.allPirates.Dlist1;

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

            if (GameSettings.game.GetMyCapsule().Holder != null)
            {
                AttackerList list = GameSettings.allPirates.Alist1;
                Pirate carrier = GameSettings.game.GetMyCapsule().Holder;
                form.RoleAssign(carrier);
                foreach (Attacker a in list)
                {
                    if (!a.Pirate.Equals(carrier) && a.Pirate.Distance(carrier) < 600)
                        form.RoleAssign(a.Pirate);
                    else if (a.Pirate.Distance(carrier) >= 600)
                        a.Pirate.Sail(carrier);
                }
                form.FormUpAndSail4Pos(GameSettings.game.GetMyMothership().Location);
            }
            else
            {
                foreach (Attacker a in GameSettings.allPirates.Alist1)
                {
                    if (!GeneralMethods.TryPush(a.Pirate))
                    {
                        a.Pirate.Sail(GameSettings.game.GetMyCapsule().Location);
                    }
                }
            }
        }
    }
}


