

using OMS.Core.Models;
using Strategy.Trader.Abstractions;

//
//   Group 88
//


namespace Strategy.Trader.Filters;

public class AllFadeFilter : AbstractStrategyFilter, IStrategyFilter
{
    public AllFadeFilter(UserProfile userProfile, ScoreCard scoreCard) : base(userProfile, scoreCard)
    {
    }

    public int IsInAlgoFilter()
    {
        return -1;
    }
}
