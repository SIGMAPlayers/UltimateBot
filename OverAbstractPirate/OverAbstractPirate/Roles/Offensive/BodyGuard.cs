using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;


namespace MyBot
{
    public class BodyGuard : BaseAttacker
    {
        
        public BodyGuard()
        {
            this.fieldAnalyzer = new FieldAnalyzer();
        }
        public BodyGuard(Pirate pirate)
        {
            this.Pirate = pirate;
        }

        public override void ExecuteCommand()
        {
            if(this.TargetEnemy == null)
            {
                if (FormationComplete)
                {
                    this.SailToTarget();
                }
                else
                {
                    this.SailToPosition();
                }
            }
            else
            {
                this.TargetedPushing(); //HELP!!! DEMENDS CARRIER WHICH BODYGUARD DOSNT KNOW!
            }
            
        }

        protected override void SailToPosition()
        {
            if (!this.AttackersTryPush())
            {
                this.Pirate.Sail(this.PositionInFormation);
            }
        }

    }
}
