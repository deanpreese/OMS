
using OMS.SharedKernel.DTO;
using Strategy.Trader.Abstractions;

namespace Strategy.Trader.Filters;

public class AllFollowFilter : IStrategyFilter
{
    ScoreCardDTO _scoreCard;

    public int IsInFilter(ScoreCardDTO scoreCard)
    {
        return 1;
    }
}
