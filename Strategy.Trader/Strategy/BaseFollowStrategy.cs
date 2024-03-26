
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Strategy.Trader.Strategy;

public class BaseFollowStrategy : AbstractStrategy , IStrategy
{

    public BaseFollowStrategy(IStrategyConnection strategyConnection) 
    {
        _filters = new List<IStrategyFilter>
        {
            new AllFollowFilter()
        };
        _strategyConnection = strategyConnection;
    }

    public override async  Task<NewOrderDTO> OnNewData(LiveOrderDTO traderLiveOrderDTO)
    {
        return await ProcessNewData(traderLiveOrderDTO);
    }

    public async override Task<int> EvaluateFilters(ScoreCardDTO scoreCard)
    {
        int includeExclude = 0;
        foreach (IStrategyFilter filter in _filters)
        {
            includeExclude = filter.IsInFilter(scoreCard);
        }
        return await Task.FromResult(includeExclude);
    }

}
