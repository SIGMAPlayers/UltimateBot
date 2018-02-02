using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;


namespace MyBot

{
    public class Carrier : BaseAttacker
    {
        public Carrier()
        {
            
        }
        public Carrier(Pirate pirate)
        {
            this.Pirate = pirate;
        }
        /// <summary>
        /// 
        /// </summary>
        public void HoldYourPosition()
        {
            this.AttackersTryPush();
        }
        public override void SailToPosition()
        {
            HoldYourPosition();
        }
        
        public override void SailToTarget()
        {
            if(!this.AttackersTryPush())
            {
                this.Pirate.Sail(Destination);
            }
        }

        public override void ExecuteCommand()
        {
            throw new NotImplementedException();
        }

    }
}
