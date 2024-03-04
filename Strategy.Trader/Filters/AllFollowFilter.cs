

using OMS.Core.Models;
using Strategy.Trader.Abstractions;

//
//   Group 90
//


namespace Strategy.Trader.Filters;

public class AllFollowFilter : AbstractStrategyFilter, IStrategyFilter
{
    public AllFollowFilter(UserProfile userProfile, ScoreCard scoreCard) : base(userProfile, scoreCard)
    {
    }

    public int IsInAlgoFilter()
    {
        return 1;
    }
}
