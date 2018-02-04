
using System.Collections.Generic;
using System.Linq;
using Pirates;


namespace MyBot

{
    public class Carrier : BaseAttacker
    {
        public Carrier()
        {
            this.fieldAnalyzer = new FieldAnalyzer();
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
        protected override void SailToPosition()
        {
            HoldYourPosition();
        }
        
        

        public override void ExecuteCommand()
        {
           
            if(FormationComplete)
            {
                this.SailToTarget();
            }
            else
            {
                this.HoldYourPosition();
            }
        }

    }
}
