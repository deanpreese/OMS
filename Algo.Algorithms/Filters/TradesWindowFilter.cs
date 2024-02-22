using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algo.Algorithms.Abstractions;
using Algo.Algorithms.Models;
using OMS.Core.Models;
using OrleansCodeGen.OMS.Core.Models;


namespace Algo.Algorithms.Filters;

public class TradesWindowFilter : AbstractBase, IAlgoFilter
{

    public TradesWindowFilter(UserProfile userProfile, ScoreCard scoreCard, AlgoData algoData) 
        : base(userProfile, scoreCard, algoData) 
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

