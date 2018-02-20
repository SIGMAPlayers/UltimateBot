using System.Collections.Generic;
using System.Linq;
using Pirates;


namespace MyBot
{
    public abstract class BaseAttacker : BaseCommand
    {
        private Pirate pirate;
        private Pirate targetEnemy;
        private Location destination;
        private Location positionInFormation;
        private static bool formationComplete;
        public FieldAnalyzer fieldAnalyzer;


        public List<MapObject> BestWay { get; set; }
        public Pirate Pirate { get => pirate; set => pirate = value; }
        public Pirate TargetEnemy { get => targetEnemy; set => targetEnemy = value; }
        public Location Destination { get => destination; set => destination = value; }
        public Location PositionInFormation { get => positionInFormation; set => positionInFormation = value; }
        public static bool FormationComplete { get => formationComplete; set => formationComplete = value; }
        public MapObject GoingTo;

        public abstract void ExecuteCommand();
        protected abstract void SailToPosition();
        protected void SailToTarget()
        {
            if(GameSettings.Game.GetLivingAsteroids().Length > 1)
                GameSettings.Game.Debug("pirate distance from asteroid "+pirate.Distance(GameSettings.Game.GetLivingAsteroids()[1]));
            if (!this.AttackersTryPush())
            {
                GoingTo = FindBestWay(this.Pirate, Destination);
                this.Pirate.Sail(GoingTo.GetLocation());
            }
        }


        #region AttackersTryPush
        protected bool AttackersTryPush()
        {
            Location upperDot = this.pirate.Location.Towards(Destination, pirate.PushRange);
            Location U = upperDot.Subtract(this.pirate.GetLocation());
            
            foreach (Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (this.pirate.CanPush(enemy))
                {

                    this.pirate.Push(enemy, pirate.Location.Add(U.Multiply(-5)));

                    GameSettings.Game.Debug("pirate " + this.pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);
                    //Did push.
                    return true;
                }

            }
            foreach (Wormhole w in GameSettings.Game.GetAllWormholes())
            {
                foreach (Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
                {
                    // Check if the pirate can push the enemy.
                    if (this.pirate.CanPush(w) && enemy.InRange(w, this.pirate.PushDistance))
                    {

                        this.pirate.Push(w, enemy.Location);

                        GameSettings.Game.Debug("pirate " + this.pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);
                        //Did push.
                        return true;
                    }

                }
                 if (this.pirate.CanPush(w) && GameSettings.Game.GetEnemyCapsules().Length > 0)
                {
                    // Push asteroid!
                    this.pirate.Push(w, GameSettings.Game.GetMyMotherships()[0]);

                    // Print a message
                    //GameSettings.Game.Debug("pirate " + pirate + " pushes " + asteroid + " towards " + GameSettings.Game.GetEnemyCapsules()[0]);

                    // Did push
                    return true;
                }
            }
            foreach (Asteroid asteroid in GameSettings.Game.GetLivingAsteroids())
            {
                // Check if the pirate can push the asteroid
                if (this.pirate.CanPush(asteroid) && GameSettings.Game.GetEnemyCapsules().Length > 0)
                {
                    // Push asteroid!
                    this.pirate.Push(asteroid, GameSettings.Game.GetEnemyCapsules()[0]);

                    // Print a message
                    //GameSettings.Game.Debug("pirate " + pirate + " pushes " + asteroid + " towards " + GameSettings.Game.GetEnemyCapsules()[0]);

                    // Did push
                    return true;
                }
            }
            // Didn't push.
            return false;
        }
        #endregion

        #region TargetedPushing
        /// <summary>
        /// takes a carrier for perspective to prtoect from enemys coming
        /// an ataacker who choose to targetPush ill lock himself on to a target and face it until it can push it.  
        /// </summary>
        /// <param name="carrier"></param>
        public void TargetedPushing(Carrier carrier)
        {
            Location upperDot = this.pirate.Location.Towards(Destination, pirate.PushRange);
            Location U = upperDot.Subtract(this.pirate.GetLocation());

            if (this.pirate.CanPush(targetEnemy))
            {
                this.pirate.Push(targetEnemy, pirate.Location.Add(U.Multiply(-5)));
                this.TargetEnemy = null;
            }
            else
                this.pirate.Sail(carrier.pirate.Location.Towards(targetEnemy, this.pirate.PushRange * 2));
        }
        #endregion
    }
}
