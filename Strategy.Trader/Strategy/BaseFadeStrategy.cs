
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;

namespace Strategy.Trader.Strategy;

public class BaseFadeStrategy : AbstractStrategy , IStrategy
{
    public BaseFadeStrategy(IStrategyConnection strategyConnection) 
    {
        _filters = new List<IStrategyFilter>
        {
            new AllFadeFilter()
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
