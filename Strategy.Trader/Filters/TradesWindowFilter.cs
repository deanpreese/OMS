using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using OMS.Core.Models;
using OrleansCodeGen;
using Strategy.Trader.Abstractions;


namespace Strategy.Trader.Filters;

public class TradesWindowFilter : AbstractStrategyFilter, IStrategyFilter
{
    public TradesWindowFilter(UserProfile userProfile, ScoreCard scoreCard) : base(userProfile, scoreCard) 
    {
    }

    public int IsInAlgoFilter()
    {
        int includeExclude = 0;

        if (_scoreCard.Trades > 5)
        {
            includeExclude = 1;
        }
        return includeExclude;
    }

}

