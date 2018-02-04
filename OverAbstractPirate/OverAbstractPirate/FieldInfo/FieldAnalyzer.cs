using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class FieldAnalyzer
    {
        //My BestFriend
        Calculator calculator;


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


        #region AssignFormationLocations
        /// <summary>
        ///     when called, the function evaluates the locations of the guardiens according to the formation shape using vectors
        /// </summary>
        private void AssignFormationLocations(List<BaseAttacker> participants)
        {
            Location guardiensPosition = new Location(0,0);
            foreach(BaseAttacker Role in participants)
            {
                if(Role is Carrier)
                {
                    guardiensPosition = calculator.CalculateVectorOfFormation(Role as Carrier);
                }
            }
            foreach(BaseAttacker Role in participants)
            {
                if(Role is Carrier)
                {
                    continue;
                }
                else
                {
                    Role.PositionInFormation = guardiensPosition;
                }
            }

        }
        #endregion

        public Mothership FindClosestMotherShip(Pirate pirate)
        {
            Mothership closestEnemyMotherShip = GameSettings.Game.GetEnemyMotherships().OrderBy(Mothership => Mothership.Location.Distance(pirate)).ToList()[0];
            return closestEnemyMotherShip;
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
                        attacker.Destination = FormCarrier.Destination.Towards(attacker.Pirate, attacker.Pirate.PushRange * 2);
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
            Carrier c = participants.OfType<Carrier>().ToList()[0];
            enemys = enemys.OrderBy(Pirate => Pirate.Distance(c.Pirate)).ToList();
            foreach(BodyGuard BG in guardians)
            {
                if(BG.TargetEnemy == null)
                {
                    BG.TargetEnemy = enemys[0];
                    enemys.RemoveAt(0);
                }
            }
        }
    }
}

