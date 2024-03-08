
using OMS.Core.Models;
using Strategy.Trader.Abstractions;

namespace Strategy.Trader.Filters;

public class AllFadeFilter :  IStrategyFilter
{
    ScoreCard _scoreCard;

    public int IsInFilter(ScoreCard scoreCard)
    {
        return -1;
    }

}
