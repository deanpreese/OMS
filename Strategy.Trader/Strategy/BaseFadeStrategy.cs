using OMS.Core.Models;
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;
using Strategy.Trader.Models;

namespace Strategy.Trader.Strategy;


public class BaseFadeStrategy : AbstractStrategy
{
    //  NG3
    // Group 88

    public BaseFadeStrategy(IGrainFactory grainFactory, StrategyAccount algoData) : base(grainFactory, algoData)
    {
    }

    public override List<IStrategyFilter> AddOrderFilters(UserProfile userProfile, ScoreCard scoreCard)
    {
        List<IStrategyFilter> _filters = new List<IStrategyFilter>
        {
            new AllFadeFilter(userProfile, scoreCard)
        };

        return _filters;
    }

    public override int CheckFilters()
    {
        int includeExclude = 0;
        foreach (IStrategyFilter filter in _filters)
        {
            includeExclude = filter.IsInAlgoFilter();
        }
        return includeExclude;
    }
}
