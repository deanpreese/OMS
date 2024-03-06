using OMS.Core.Interfaces;
using OMS.Core.Models;
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;
using Strategy.Trader.Models;


namespace Strategy.Trader.Strategy;


public class BaseFollowStrategy : AbstractStrategy
{

    public BaseFollowStrategy(IGrainFactory grainFactory, StrategyAccount algoData) : base(grainFactory, algoData)
    {
    }

    public override List<IStrategyFilter> AddFilters()
    {
        List<IStrategyFilter> _filters = new List<IStrategyFilter>
        {
            new AllFollowFilter()
        };

        return _filters;
    }

    public override Task<int> EvaluateFilters(string trader_key, string strategy_key, ITraderGrain traderGrain, IStrategyGrain strategyGrain)
    {
        int includeExclude = 0;
        foreach (IStrategyFilter filter in _filters)
        {
            includeExclude = filter.IsInFilter();
        }
        return Task.FromResult(includeExclude);
    }
}
