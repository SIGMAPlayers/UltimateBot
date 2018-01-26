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
            AttackersTryPush(this.Pirate);
        }

        public override void ExecuteCommand()
        {
            throw new NotImplementedException();
        }
    }
}
