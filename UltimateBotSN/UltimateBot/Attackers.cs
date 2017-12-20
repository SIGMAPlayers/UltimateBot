using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pirates;

namespace MyBot
{
    public enum Attackers { Collector, BodyGuard, TailGuard };
    //An attacker pirate
    public class Attacker
    {
        private Pirate pirate;
        private Attackers duty;

        public Pirate Pirate { get => pirate; set => pirate = value; }
        public Attackers Duty { get => duty; set => duty = value; }

        public Attacker(Pirate pirate, Attackers role)
        {
            this.duty = role;
            this.pirate = pirate;
        }

        public void SwitchRoles(Attacker a, Attacker b)
        {
            Attackers tempDuty = b.duty;
            b.duty = a.duty;
            a.duty = tempDuty;
        }

        public void BecomeCollector(Attacker replacer, Attacker Collector)
        {
            if (replacer.pirate.IsAlive())
            {
                SwitchRoles(replacer, Collector);
            }
        }

        public bool IsAlive()
        {
            return this.pirate.IsAlive();
        }
    }
}
