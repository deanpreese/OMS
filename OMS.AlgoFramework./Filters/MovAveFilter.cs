using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMS.AlgoManager.Filters;

public class MovAveFilter
{

    public double LastPNL = 0;
    public double LastAverage = 0;


    public int CalculateLeverageModifier(string trader, int length)
    {
        int leverageMod = 0;

        /*
        TraderGroupRanking tgr = new TraderGroupRanking(trader);
        double pct = tgr.RankingPct;

        int ab = CalculateAveValues(trader, length);

        if (tgr.CurrentGroupRank == 0)
        {
            leverageMod = 0;
        }
        else
        {

            // MA < PNL
            if (pct > 0.9 && ab == -1)
            {
                leverageMod = 3;
            }

            if (pct < 0.9 && pct > 0.8 && ab == -1)
            {
                leverageMod = 2;
            }

            if (pct < 0.8 && pct > 0.7 && ab == -1)
            {
                leverageMod = 1;
            }

            if (pct < 0.7 && pct > 0.1 && ab == -1)
            {
                leverageMod = 0;
            }

            if (pct < 0.1 && ab == -1)
            {
                leverageMod = 3;
            }


            // MA > PNL
            if (pct > 0.9 && ab == 1)
            {
                leverageMod = -3;
            }

            if (pct < 0.9 && pct > 0.8 && ab == 1)
            {
                leverageMod = -2;
            }

            if (pct < 0.8 && pct > 0.7 && ab == 1)
            {
                leverageMod = -1;
            }

            if (pct < 0.7 && pct > 0.1 && ab == 1)
            {
                leverageMod = 0;
            }

            if (pct < 0.1 && ab == 1)
            {
                leverageMod = -3;
            }
        }
        */

        return leverageMod;
    }


    private int CalculateAveValues(string trader, int length)
    {
        int aboveBelow = 0;

        /*
        AlgoMovingAverage ama = new AlgoMovingAverage(trader, length);

        LastPNL = ama.LastPNL;
        LastAverage = ama.LastAverage;

        if ( ama.LastAverage > ama.LastPNL  )
        {
            aboveBelow = 1;
        }

        if (ama.LastAverage < ama.LastPNL)
        {
            aboveBelow = -1;
        }


        if (ama.LastPNL < 0
            && ama.LastPNL < ama.LastAverage)
        {
            aboveBelow = 1;
        }
        */
        return aboveBelow;
    }



}

