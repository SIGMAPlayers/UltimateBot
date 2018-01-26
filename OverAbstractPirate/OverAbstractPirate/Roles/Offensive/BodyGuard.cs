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

        }
        public BodyGuard(Pirate pirate)
        {
            this.Pirate = pirate;
        }

        public override void ExecuteCommand()
        {
            throw new NotImplementedException();
        }
    }
}
