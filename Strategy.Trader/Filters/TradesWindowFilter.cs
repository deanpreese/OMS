using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using OMS.Core.Models;
using OrleansCodeGen;
using Strategy.Trader.Abstractions;


namespace Strategy.Trader.Filters;

public class TradesWindowFilter : IStrategyFilter
{
    private UserProfile _userProfile;
    private ScoreCard _scoreCard;


    public void UpdateFilter(UserProfile userProfile, ScoreCard scoreCard)
    {
        _userProfile = userProfile;
        _scoreCard = scoreCard;
    }
    public int IsInFilter()
    {
        int includeExclude = 0;

        if (_scoreCard.Trades > 5)
        {
            includeExclude = 1;
        }
        return includeExclude;
    }

}

