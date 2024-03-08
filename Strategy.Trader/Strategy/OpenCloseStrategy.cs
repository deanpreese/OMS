using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Core.Models;
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;
using Strategy.Trader.Models;

namespace Strategy.Trader.Strategy;

public class OpenCloseStrategy : StrategyBase , IStrategy
{
    IClusterClient newClusterClient;

    int orderCount = 1;



    public OpenCloseStrategy(IClusterClient clusterClient, StrategyAccount strategyData) 
    {
        newClusterClient = clusterClient;
        _filters = new List<IStrategyFilter>
        {
            new TwoSidedFilter()
        };

        _strategyData = strategyData;
    }



    public override Task<NewOrder> OnNewOrder(LiveOrder _orig_live_order)
    {
        throw new NotImplementedException();
    }



    public override async Task<int> EvaluateFilters(string trader_key, string strategy_key, 
        ITraderInfoGrain traderGrain, IStrategyGrain strategyGrain)
    {
        int includeExclude = 0;

        ScoreCard _scoreCard = await traderGrain.GetScoreCardAsync(trader_key);            

        foreach (IStrategyFilter filter in _filters)
        {
            includeExclude = filter.IsInFilter(_scoreCard);
        }

        return includeExclude;
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
