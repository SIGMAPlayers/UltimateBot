using System.Collections.Generic;
using System.Linq;
using Pirates;


namespace MyBot
{
    public class BodyGuard : BaseAttacker
    {
        Carrier GuardedCarrier;
        public BodyGuard()
        {
            this.fieldAnalyzer = new FieldAnalyzer();
        }
        public BodyGuard(Pirate pirate)
        {
            this.Pirate = pirate;
        }

        public void assignCarrier(Carrier c)
        {
            GuardedCarrier = c;
           
            GameSettings.Game.Debug("Guarded carrier is ===> " + c.Pirate.Id);
        }

        public override void ExecuteCommand()
        {
            if(this.TargetEnemy == null)
            {
                if (FormationComplete)
                {
                    GameSettings.Game.Debug("Formation Clomlete lets sail to the target ==> "+ this.Destination);
                    this.SailToTarget();
                }
                else
                {
                    GameSettings.Game.Debug("Formation in incomplete les sail to the position ==>" + this.PositionInFormation);
                    this.SailToPosition();
                }
            }
            else
            {
                this.TargetedPushing(GuardedCarrier);
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
