using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algo.Trader.Abstractions;
using Algo.Trader.Models;
using OMS.Core.Models;
using OrleansCodeGen;


namespace Algo.Trader.Filters;

public class TradesWindowFilter : AbstractAlgoFilter, IAlgoFilter
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

