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
        // Changed Pirates to defenders where needed &
        // Changed the defenders to two layer code
        //a function to start the game
        public AllPirates OnGameStart(PirateGame game)
        {
            AllPirates allPirates = new AllPirates(game);
            return allPirates;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public void DoTurn(PirateGame game)
        {
            Formation form = null;
            GameSettings.game = game;
            if (!GameSettings.START)
            {
                GameSettings.game = game;

                GameSettings.allPirates = OnGameStart(game);
                GameSettings.defList = GameSettings.allPirates.DefenderList;
                GameSettings.AtkList = GameSettings.allPirates.AttackerList;
                form = new Formation();

                GameSettings.START = true;
            }

            List<Defender> defenders = GameSettings.allPirates.DefenderList;

            foreach (Defender defender in defenders)
            {
                if (defender.IsAlive())
                {
                    if (!defender.TryPush(defender, game))
                    {
                        // Enemy capsule defenders work
                        Location start;
                        if ((defenders[0].Equals(defender) || defenders[1].Equals(defender)))
                        {
                            game.Debug("Entered defender 0 / 1");
                            defender.Layer = Roles.front;
                            Pirate pirate = defender.DefendFrom(game);
                            game.Debug("Pirate to DefendFrom: "+pirate);
                            if (pirate == null)
                            {
                                start = defender.ProtectFromCarriers(0, game);
                                defender.Pirate.Sail(start);
                            }
                            else
                                defender.Pirate.Sail(pirate);
                        }
                        else //if (defenders[2].Equals(defender) || defenders[3].Equals(defender))
                        {
                            game.Debug("Entered defender 2 / 3");
                            defender.Layer = Roles.backup;
                            Pirate pirate = defender.DefendFrom(game);
                            if (pirate == null)
                            {
                                start = defender.ProtectFromCarriers(450, game);
                                defender.Pirate.Sail(start);
                            }
                            else
                                defender.Pirate.Sail(pirate);
                        }
                    }
                }
            }

            if (GameSettings.game.GetMyCapsule().Holder != null)
            {
                AttackerList list = GameSettings.allPirates.AttackerList;
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
                foreach (Attacker a in GameSettings.allPirates.AttackerList)
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