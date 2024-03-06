using OMS.Core.Interfaces;
using OMS.Core.Models;
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;
using Strategy.Trader.Models;

namespace Strategy.Trader.Strategy;

public class OpenCloseStrategy : AbstractStrategy
{
    public OpenCloseStrategy(IGrainFactory grainFactory, StrategyAccount accountData) : base(grainFactory, accountData) {}

    public override List<IStrategyFilter> AddFilters()
    {
        List<IStrategyFilter> _filters = new List<IStrategyFilter>
        {
            new TwoSidedFilter()
        };
        return _filters;
    }

    public override async Task<int> EvaluateFilters(string trader_key, string strategy_key, ITraderGrain traderGrain, IStrategyGrain strategyGrain)
    {
        int includeExclude = 0;

        UserProfile _userProfile = await traderGrain.GetProfileAsync(trader_key);
        ScoreCard _scoreCard = await traderGrain.GetScoreCardAsync(trader_key);            

        foreach (IStrategyFilter filter in _filters)
        {
            filter.UpdateFilter(_userProfile, _scoreCard);
            includeExclude = filter.IsInFilter();
        }

        return includeExclude;
    }
}
