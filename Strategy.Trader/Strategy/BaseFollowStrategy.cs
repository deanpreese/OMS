using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Core.Models;
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;
using Strategy.Trader.Models;


namespace Strategy.Trader.Strategy;


public class BaseFollowStrategy : AbstractStrategy
{
    IClusterClient newClusterClient;
    int orderCount = 1;

    public BaseFollowStrategy(IClusterClient clusterClient, StrategyAccount strategyData) 
    : base(clusterClient, strategyData)
    {
        newClusterClient = clusterClient;
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


    public async override Task ProcessOrderForStrategy(string strategy_key, NewOrder order)
    {
        if(order.OrderAction != OrderAction.NoAction)
        {
            IOrderGrain orderGrain = newClusterClient.GetGrain<IOrderGrain>("T"+_strategyData.strategy_traderId + "-"+ orderCount);
            await orderGrain.ProcessOrder(order);
            orderCount++;
        }
    }
}
