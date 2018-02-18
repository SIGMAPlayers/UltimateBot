using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class FieldAnalyzer
    {
        //My BestFriend
        Calculator calculator;
        
        public FieldAnalyzer()
        {
            calculator = new Calculator();
        }

        /// <summary>
        /// checks if there is any threat on the given pirate within a given range. sorted by distance from pirate.
        /// what is a threat:
        /// -inside the range of danger.
        /// -not 300+ units behind the pirate.
        /// -coming towards me
        /// </summary>
        /// <param name="pirate"></param>
        /// <param name="rangeOfDanger"></param>
        /// <returns>list of piretes </returns>
        public List<Pirate> UnderThreat(Pirate pirate, int rangeOfDanger, Location destination)
        {
            List<Pirate> threatingPirates = new List<Pirate>();
            foreach (Pirate enemy in GameSettings.Game.GetEnemyLivingPirates())
            {
                int angleOfAttack = calculator.CalculateAngleOfAttack(pirate, enemy, destination);
                if (enemy.InRange(pirate, rangeOfDanger) && (angleOfAttack > 200 || angleOfAttack < 160))
                    threatingPirates.Add(enemy);

            }
            return threatingPirates;
        }

        public bool IsFormationGuardsCloseToTheCarrier(List<ICommand> form, Carrier carrier)
        {
            foreach(BaseAttacker guard in form.Cast<BaseAttacker>().ToList())
            {
                if(!(guard is Carrier))
                {
                    if(guard.Pirate.InRange(carrier.Pirate, 1200) && guard.PositionInFormation != guard.Pirate.Location)
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }
        
        #region AssignFormationLocations
        /// <summary>
        ///     when called, the function evaluates the locations of the guardiens according to the formation shape using vectors
        /// </summary>
        public void AssignFormationLocations(List<BaseAttacker> participants)
        {
            if(participants.Count > 0)
            {
                Location guardiensPosition = calculator.CalculateBEstCapsuleToGoTo(participants[0].Pirate); 
                foreach(BaseAttacker Role in participants)
                {
                    if(Role is Carrier)
                    {
                        guardiensPosition = calculator.CalculateVectorOfFormation(Role as Carrier);
                    }
                    else
                    {
                    
                    }
                }
               
                foreach(BaseAttacker Role in participants)
                {
                    if(Role is Carrier)
                    {
                        Role.PositionInFormation = Role.Pirate.GetLocation();
                    }
                    else
                    {
                        if(Role.GoingTo is Wormhole)
                        {
                            Role.PositionInFormation = (Role as BodyGuard).GuardedCarrier.Pirate.GetLocation();
                        }
                        else
                            Role.PositionInFormation = guardiensPosition;
                    }
                }
            }
        }
        #endregion

        public Mothership FindClosestMotherShip(Pirate pirate)
        {
            Mothership closestmyMotherShip = GameSettings.Game.GetMyMotherships().OrderBy(Mothership => Mothership.Location.Distance(pirate)).ToList()[0];
            return closestmyMotherShip;
        }
        
        public int HowMuchCapsules (PirateGame game, List<Pirate> pirates)
        {
            int count = 0;
            foreach (Pirate pirate in pirates)
            {
                if (pirate.HasCapsule())
                {
                    count++;
                }
            }
            return count;
        }
        
        public void DefineTargets(List<BaseAttacker> participants)
        {
            Carrier FormCarrier = null;
            foreach(BaseAttacker attacker in participants)
            {
                if (attacker is Carrier)
                {
                    FormCarrier = attacker as Carrier;
                }
            }
            if(FormCarrier != null)
            {
                
                FormCarrier.Destination = FindClosestMotherShip(FormCarrier.Pirate).Location;
                foreach(BaseAttacker attacker in participants)
                {
                    if(attacker is Carrier)
                    {
                        
                    }
                    else
                    {
                        attacker.Destination = FormCarrier.Destination;
                    }
                }
            }
            else
            {
                foreach(BaseAttacker attacker in participants)
                {
                    
                    attacker.Destination = calculator.CalculateBEstCapsuleToGoTo(attacker.Pirate);
                }
            }

            
        }

        public void PopulateEnemyTargets(List<Pirate> enemys, List<BaseAttacker> participants)
        {
            List<BodyGuard> guardians = participants.OfType<BodyGuard>().ToList();
            GameSettings.Game.Debug("BodyGuards are ==> " + guardians.Count);
            Carrier c = participants.OfType<Carrier>().ToList()[0];
            GameSettings.Game.Debug("Carrier is ==> " + c);
            enemys = enemys.OrderBy(Pirate => Pirate.Distance(c.Pirate)).ToList();
            foreach(BodyGuard BG in guardians)
            {
                if(BG.TargetEnemy == null)
                {
                    if(enemys.Count > 0)
                    {
                        BG.TargetEnemy = enemys[0];
                    enemys.RemoveAt(0);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Returns how many carriers are near the Mothership and can be double pushed
        /// </summary>
        /// <param name="defenderList"></param>
        /// <returns></returns>
        public List<BaseDefender> CheckHowManyDefendrsCanPushEnemyCarrier(List<Pirate> enemyCarriers, List<BaseDefender> defenders)
        {
            List<BaseDefender> canDoublePush = new List<BaseDefender>();

            foreach(Pirate enemyCarrier in enemyCarriers)
            {
                canDoublePush = new List<BaseDefender>();

                foreach (BaseDefender defender in defenders)
                {
                    if (defender.Pirate.CanPush(enemyCarrier))
                    {
                        canDoublePush.Add(defender);
                    }
                }
                if (canDoublePush.Count > 1)
                {
                    return canDoublePush;
                }
            }
            return new List<BaseDefender>();
        }
        
        /// <summary>
        /// Returns how many carriers are near the Mothership and can be double pushed
        /// </summary>
        /// <param name="defenderList"></param>
        /// <returns></returns>
        public List<Pirate> HowManyCarriersNearCityCanBeDoublePushed(List<BaseDefender> defenderList)
        {
            List<Pirate> carriers = CarrierCloseToCity();
            List<Pirate> canBeDoublePushed = new List<Pirate>();
            int countCanPush = 0;

            foreach (Pirate enemyCarrier in carriers)
            {
                countCanPush = 0;

                foreach (BaseDefender defender in defenderList)
                {
                    if (defender.Pirate.CanPush(enemyCarrier))
                    {
                        countCanPush++;
                    }
                }

                if (countCanPush > 1)
                {
                    canBeDoublePushed.Add(enemyCarrier);
                }
            }

            return canBeDoublePushed;
        }
        
        /// <summary>
        /// Get the most closest enemy carrier to a city
        /// </summary>
        /// <param name="mothershipToProtect"></param>
        /// <returns></returns>
        public Pirate GetMostThreatningEnemyCarrier(Mothership mothershipToProtect)
        {
            foreach(Pirate pirate in GameSettings.Game.GetEnemyLivingPirates())
            {
                if(pirate.HasCapsule() && pirate.Distance(mothershipToProtect) < pirate.PushDistance * 1.5)
                {
                    //GameSettings.Game.Debug("Most threating EC = "+pirate);
                    return pirate;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Returns the closest capsule to a mothership
        /// </summary>
        /// <param name="mothership"></param>
        /// <returns></returns>
        public Capsule GetClosestEnemyCapsuleToMothership(Mothership mothership)
        {
            int minDistance = 100000;
            Capsule minCapsule = null;

            foreach(Capsule capsule in GameSettings.Game.GetEnemyCapsules())
            {
                if(capsule.InitialLocation.Distance(mothership) < minDistance)
                {
                    minDistance = capsule.InitialLocation.Distance(mothership);
                    minCapsule = capsule;
                }
            }

            return minCapsule;
        }

        /// <summary>
        /// Returns a list of carriers that are close to their city
        /// </summary>
        /// <returns></returns>
        private List<Pirate> CarrierCloseToCity()
        {
            List<Pirate> closeCarriers = new List<Pirate>();

            foreach(Pirate enemyPirate in GameSettings.Game.GetEnemyLivingPirates())
            {
                if(enemyPirate.HasCapsule())
                {
                    Mothership closestEnemyMotherShip = GameSettings.Game.GetEnemyMotherships().OrderBy(Mothership => Mothership.Location.Distance(enemyPirate)).ToList()[0];
                    if (enemyPirate.Distance(closestEnemyMotherShip) < enemyPirate.PushDistance * 2)
                    {
                        closeCarriers.Add(enemyPirate);
                    }
                }
            }
            return closeCarriers;
        }

        /// <summary>
        /// Defines where the defender need to push
        /// </summary>
        /// <param name="enemyPirate"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public Location DefendersWhereToPush(Pirate enemyPirate, int range)
        {
            Location outOfBorder = GetCloseEnoughToBorder(enemyPirate, range);
            if (outOfBorder != null)
            {
                return outOfBorder;
            }
            else
            {
                Location oppositeSide = enemyPirate.GetLocation().Subtract(GameSettings.Game.GetEnemyMotherships()[0].GetLocation());
                //Vector: the distance (x,y) you need to go through to go from the mothership to the enemy
                return oppositeSide = enemyPirate.GetLocation().Towards(enemyPirate.GetLocation().Add(oppositeSide), 600);
            }
        }

        public List<Pirate> GetClosestEnemyPiratesToMothership(BaseDefender defender)
        {
            List<Pirate> closestEnemyPirates = new List<Pirate>();
            Mothership closestEnemyMotherShip = GameSettings.Game.GetEnemyMotherships().OrderBy(Mothership => Mothership.Location.Distance(defender.Pirate)).ToList()[0];

            foreach (Pirate pirate in GameSettings.Game.GetEnemyLivingPirates())
            {
                if(pirate.Distance(closestEnemyMotherShip) < defender.Pirate.PushDistance * 3.5)
                {
                    closestEnemyPirates.Add(pirate);
                }
            }

            return closestEnemyPirates;
        }

        /// <summary>
        /// Predict the Location of a SpaceObject in number of turns
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="turns"></param>
        /// <returns></returns>
        public Location PredictLocation(SpaceObject obj, int turns)
        {
            return calculator.PredictLocationByMovement(obj, turns);
        }

        /// <summary>
        /// Checks if the enemy pirate is close enough to the border to kill him. 
        /// Returns the location that if you push it towards it, the pirate will die or null if you can't kill it.
        /// </summary>
        /// <param name="enemyPirate">The enemy pirate to be checked.</param>
        /// <param name="range">The range that will be checked if you can throw it</param>
        /// <returns>Returns the location that if you push it towards it, the pirate will die or null if you can't kill it.</returns>
        public Location GetCloseEnoughToBorder(Pirate enemyPirate, int range)
        {
            Location up = new Location(0, enemyPirate.Location.Col);
            Location right = new Location(enemyPirate.Location.Row, GameSettings.Game.Cols);
            Location left = new Location(enemyPirate.Location.Row, 0);
            Location down = new Location(GameSettings.Game.Rows, enemyPirate.Location.Col);
            int upDistance = enemyPirate.Distance(up);
            int rightDistance = enemyPirate.Distance(right);
            int leftDistance = enemyPirate.Distance(left);
            int downDistance = enemyPirate.Distance(down);

            if (upDistance < rightDistance && upDistance < leftDistance && upDistance < downDistance)
            {
                if (upDistance < range)
                    return up;
            }
            else if (rightDistance < upDistance && rightDistance < leftDistance && rightDistance < downDistance)
            {
                if (rightDistance < range)
                    return right;
            }
            else if (leftDistance < upDistance && leftDistance < rightDistance && leftDistance < downDistance)
            {
                if (leftDistance < range)
                    return down;
            }
            else if (downDistance < upDistance && downDistance < rightDistance && downDistance < leftDistance)
            {
                if (downDistance < range)
                    return left;
            }
            //Returns null if not close enough to a border
            return null;
        }

        /// <summary>
        /// Checks if an enemy will be close to my attacker after I push myself to a Location
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public bool CheckWhetherEnemyIsCloseToMeAfterPush (BaseAttacker attacker, Pirate enemy)
        {
            Location enemyGoingTo = calculator.PredictLocationByMovement(enemy, 1);
            Location pushTo = calculator.PredictLocationAfterPush(attacker.Pirate, attacker.Destination, attacker.Destination);
            Pirate newEnemy = new Pirate();
            newEnemy.Location = pushTo;

            if (newEnemy.InPushRange(pushTo))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Checks how many GameObjects are near another GameObject by distance
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="fromObj"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public int CheckHowManyGameObjectsNearAreaByDistance(List<GameObject> gameObjects, GameObject fromObj, int distance)
        {
            int count = 0;
            foreach (GameObject gameObject in gameObjects)
            {
                if (calculator.CheckIfCloseToObjectByDistance(gameObject, fromObj, distance))
                    count++;
            }
            return count;
        }
        
        /// <summary>
        /// Gets the most populated with defenders enemy mothership, returns null if there is no mothership
        /// </summary>
        /// <returns></returns>
        public Mothership GetMostPopulatedEnemyMothership()
        {
            List<Mothership> mothershipList = GameSettings.Game.GetEnemyMotherships().ToList();

            int countMaxPirates = 0;
            int countPirates = 0;
            int iMaxMothership = 0;

            for (int i = 0; i < mothershipList.Count; i++)
            {
                countPirates = 0;

                foreach(Pirate enemyPirate in GameSettings.Game.GetEnemyLivingPirates())
                {
                    if(mothershipList[i].Distance(enemyPirate) < 1500)
                    {
                        countPirates++;
                    }
                }

                if(countMaxPirates < countPirates)
                {
                    countMaxPirates = countPirates;
                    iMaxMothership = i;
                }
            }

            if(mothershipList.Count > 0)
            {
                return mothershipList[0];
            }

            return null;
        }
        
         public bool isPortalDangerous(Wormhole wormhole)
        {
            int count = 0;
            foreach(Pirate p in GameSettings.Game.GetEnemyLivingPirates())
            {
                if(p.Distance(wormhole) <= p.PushRange)
                {
                    if(count > 1)
                    {
                        return true;
                        
                    }
                    else
                    {
                        count++;
                        
                    }
                }
                
            }
            return false;
            
        }
        
        public List<MapObject> GetSafeHoles()
        {
            List<Wormhole> AllHoles = GameSettings.Game.GetAllWormholes().ToList();
            List<MapObject> SafeHoles = new List<MapObject>();
            
            foreach(Wormhole wormhole in AllHoles)
                {
                    if(!isPortalDangerous(wormhole) && wormhole.IsActive )
                    {
                        SafeHoles.Add(wormhole);
                    }
                }
            
           return SafeHoles;
             
        }
        
        //use this function to move in the shortest way if that have a wormhole
        public List<MapObject> GetBestHoles(Pirate carrier, MapObject target)
        {
            GameSettings.Game.Debug("OKOKOK");
            List<MapObject> BestHoles = new List<MapObject>();
            checkPath(target, carrier.Location, BestHoles);
            GameSettings.Game.Debug(PrintPath(BestHoles));
            return BestHoles;
            
        }
        
        public string PrintPath(List<MapObject> path)
        {
            string s = "";
            foreach(MapObject l in path)
            {
                if (l is Wormhole)
                {
                    Wormhole z = l as Wormhole;
                    s+= "Wormhole number " + z.Id + "===> ";
                }
                else
                {
                    s+= "target " + l;
                }
            }
            return s;
        }
        
        public List<MapObject> checkPath(MapObject target, Location step, List<MapObject> BestWay)
        {
                List<MapObject> SafeHoles = GetSafeHoles();
                foreach (Wormhole w in SafeHoles.Cast<Wormhole>().ToList())
                {
                    if ((step.Distance(target) 
                    > step.Distance(w) 
                    + w.Partner.Distance(target)))
                        {
                           checkPath(target,w.Partner.Location, BestWay);
                           BestWay.Add(w);
                           return BestWay;
                        }
           
                }
             BestWay.Add(target);
             return BestWay;
            
        }

    }
}

