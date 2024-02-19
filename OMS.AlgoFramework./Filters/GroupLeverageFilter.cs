using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMS.AlgoManager.Filters
{
    public class GroupLeverageFilter
    {

        public int GroupRank = 0;
        public int GroupCount = 0;
        public double GroupPercentage = 0.0;
        public int GroupContraRating = 0;


        
        public int CalculateGroupLeverage(string trader)
        {
            int grpLeverage = 1;
            GroupContraRating = -1;
            return grpLeverage;
        }

        

        
        public int CalculateGroupLeverageV2(string trader)
        {
            int grpLeverage = 0;


            /*
            TraderGroupRanking tgr = new TraderGroupRanking(trader);
            double pct = tgr.RankingPct;
            GroupPercentage = pct;
            GroupCount = tgr.GroupCount;
            GroupRank = tgr.CurrentGroupRank;

            if (pct > 0.9)
            {
                GroupContraRating = -1;
                grpLeverage = 10;
            }

            if (pct < 0.9 && pct > 0.8)
            {
                GroupContraRating = -1;
                grpLeverage = 6;
            }

            if (pct < 0.8 && pct > 0.7)
            {
                GroupContraRating = -1;
                grpLeverage = 3;
            }

            if (pct < 0.7 && pct > 0.1)
            {
                
                grpLeverage = 0;
            }


            if (pct < 0.7 && pct > 0.1)
            {
                GroupContraRating = -1;
                grpLeverage = 1;
            }

            if (pct < 0.1)
            {
                GroupContraRating = 1;
                grpLeverage = 10;
            }


            if (tgr.CurrentGroupRank == 0)
            {
                GroupContraRating = -1;
                grpLeverage = 1;
            }
            */
            
            return grpLeverage;
        }

               


    }
}
