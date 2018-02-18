using System.Collections.Generic;
using System.Linq;

using Pirates;

namespace MyBot
{
    public class DeathBallAttacker : BaseAttacker
    {
        private static bool pushedAsteroid = false;
        private Asteroid closestAsteroid;

        public Asteroid ClosestAsteroid { get => closestAsteroid; set => closestAsteroid = value; }
        public bool PushedAsteroid { get => pushedAsteroid; set => pushedAsteroid = value; }

        public DeathBallAttacker(Pirate pirate) : base(pirate)
        {
            Asteroid minAsteroid = null;
            int minDistance = 100000;

            foreach (Asteroid asteroid in GameSettings.Game.GetLivingAsteroids())
            {
                if (Pirate.Distance(asteroid) < minDistance)
                {
                    minAsteroid = asteroid;
                    minDistance = Pirate.Distance(asteroid);
                }
            }

            closestAsteroid = minAsteroid;
        }

        public override void ExecuteCommand()
        {
            SailToPosition();
        }

        protected override void SailToPosition()
        {
            if (!AsteroidTryPush())
            {
                // if (!this.AttackersTryPush())
                {
                    this.Pirate.Sail(closestAsteroid);
                }
            }
        }

        public bool AsteroidTryPush()
        {
            if (closestAsteroid != null)
            {
                if (Pirate.CanPush(closestAsteroid))
                {
                    Pirate.Push(closestAsteroid, GameSettings.Game.GetEnemyMotherships()[0]);
                    pushedAsteroid = true;
                    return true;
                }
            }
            return false;
        }
    }
}
