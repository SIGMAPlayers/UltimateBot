using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class StrategyOrganizer
    {
        private List<Pirate> piratesToDeliver;
        private List<Strategy> strategyList;
        private float assignationRatio;

        public void SetAssignationRatio(FieldAnalyzer FieldAnalyzer)
        {
            //For now
            assignationRatio = 1 / strategyList.Count;
        }

        public void DeliverPirates()
        {

        }

    }
}
