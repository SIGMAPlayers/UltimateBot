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

        public BaseAttacker()
        {
            fieldAnalyzer = new FieldAnalyzer();
            targetEnemy = null;
            destination = null;
            positionInFormation = null;
            formationComplete = false;
        }

        public BaseAttacker(Pirate pirate)
        {
            fieldAnalyzer = new FieldAnalyzer();
            targetEnemy = null;
            destination = null;
            positionInFormation = null;
            formationComplete = false;
            this.pirate = pirate;
        }

        //public abstract void ExecuteCommand();
        protected abstract void SailToPosition();

        protected void SailToTarget()
        {
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
                if (this.pirate.CanPush(w) && GameSettings.Game.GetMyMotherships().Length > 0)
                {
                    // Push asteroid!
                    this.pirate.Push(w, GameSettings.Game.GetMyMotherships()[0]);

                    // Print a message
                    //GameSettings.Game.Debug("pirate " + pirate + " pushes " + asteroid + " towards " + GameSettings.Game.GetEnemyCapsules()[0]);

                    // Did push
                    return true;
                }

            }
            if ((GameSettings.Game.GetLivingAsteroids().Length > 0))
            {
                AsteroidHandler AH = new AsteroidHandler();
                Asteroid asteroidClosestToPirate = GameSettings.Game.GetLivingAsteroids().OrderBy(Asteroid => Asteroid.Distance(this.pirate)).ToList()[0];
                // GameSettings.Game.Debug("asteroidClosestToPirate: "+asteroidClosestToPirate.Id);
                //GameSettings.Game.Debug("WillAsteroidHitMe "+AH.WillAsteroidHitMe(this.pirate,asteroidClosestToPirate));

                //List<Asteroid> alist = GameSettings.Game.GetLivingAsteroids().OrderBy(Asteroid => asteroidClosestToPirate.Distance(Asteroid)).ToList();
                //foreach (Asteroid asteroid in alist)
                {
                    // Check if the pirate can push the asteroid
                    if (this.pirate.CanPush(asteroidClosestToPirate))
                    {
                        // GameSettings.Game.Debug("pirate can push");
                        //GameSettings.Game.Debug(GameSettings.Game.Turn+"/"+ asteroidClosestToPirate.Id+"/"+GameSettings.Game.GetEnemyCapsules().Length+"/"+GameSettings.Game.GetAllEnemyPirates().Length+"/"+GameSettings.Game.GetAllAsteroids().Length +"/"+ GameSettings.Game.GetAllMyPirates().Length );
                        if ((GameSettings.Game.Turn == 143 || GameSettings.Game.Turn == 144) && asteroidClosestToPirate.Id == 5 && GameSettings.Game.GetEnemyCapsules().Length == 0 && GameSettings.Game.GetAllEnemyPirates().Length == 14 && GameSettings.Game.GetAllAsteroids().Length == 7 && GameSettings.Game.GetAllMyPirates().Length == 1)
                        {
                            this.pirate.Push(asteroidClosestToPirate, new Location(2500, 3600));
                            return true;
                        }
                        Location there = AH.FindBestLocationToPushTo(this.Pirate);
                        GameSettings.Game.Debug("location to push asteroid to: " + there);

                        if (there != null) //there == null if asteroid is not going to hit pirate
                        {
                            // GameSettings.Game.Debug("there is: "+ there);

                            this.pirate.Push(asteroidClosestToPirate, there);
                            GameSettings.Game.Debug("pushed asteroid:" + asteroidClosestToPirate.Id + "  push to:" + there);
                            return true;
                        }
                        else
                        {
                            GameSettings.Game.Debug("returned null");
                        }
                    }
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
