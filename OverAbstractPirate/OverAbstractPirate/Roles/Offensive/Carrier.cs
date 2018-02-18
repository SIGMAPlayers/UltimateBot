using System.Collections.Generic;
using System.Linq;
using Pirates;


namespace MyBot

{
    public class Carrier : BaseAttacker
    {
        List<ICommand> myform;
        public Carrier(List<ICommand> form)
        {
            this.fieldAnalyzer = new FieldAnalyzer();
            this.myform = form;
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
           if(!this.AttackersTryPush())
            {
                foreach(Wormhole w in GameSettings.Game.GetInactiveWormholes())
                {
                    if(this.Pirate.InRange(w,w.WormholeRange))
                    {
                        this.Pirate.Sail(w.Location.Towards(Destination,w.WormholeRange));
                    }
                }
            }
        }
        protected override void SailToPosition()
        {
            HoldYourPosition();
        }
        
        

        public override void ExecuteCommand()
        {
           if(fieldAnalyzer.UnderThreat(this.Pirate,1200, this.Destination).Count == 0)
           {
            if(fieldAnalyzer.IsFormationGuardsCloseToTheCarrier(myform,this))
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
            else
            {
                this.SailToTarget();    
            }
           }
           else
           {
               this.SailToTarget();
           }
            
        }

    }
}
