using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pirates;

namespace MyBot
{
   
 
    public class Attacker
    {
        private Pirate pirate;
        private Pirate enemyTarget;

        public Pirate Pirate { get => pirate; set => pirate = value; }
        public Pirate EnemyTarget { get => enemyTarget; set => enemyTarget = value; }

        public Attacker(Pirate pirate)
        {
            this.Pirate = pirate;
        }
        public bool IsAlive()
        {
            return this.Pirate.IsAlive();
        }
        public void ClearEnemyTarget()
        {
            EnemyTarget = null;
        }
    }
}
