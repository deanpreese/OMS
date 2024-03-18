using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Core.Models;
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;
using Strategy.Trader.Models;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.SharedKernel.Grains;

namespace Strategy.Trader.Strategy;

public class BaseFadeStrategy : AbstractStrategyBase , IStrategy
{
    IClusterClient newClusterClient;
    int orderCount = 1;

    public BaseFadeStrategy(IClusterClient clusterClient,  StrategyAccount strategyData) 
    {
        newClusterClient = clusterClient;
        _filters = new List<IStrategyFilter>
        {
            new AllFadeFilter()
        };
        _strategyData = strategyData;
    }

    public override Task<NewOrderDTO> OnNewOrder(LiveOrder _orig_live_order)
    {
         string _trader_key = _orig_live_order.UserID + "_" + _orig_live_order.GroupID;
        _strategy_key = _strategyData.strategy_traderId + "_" + _strategyData.group;

        ITraderInfoGrain traderInfoGrain = newClusterClient.GetGrain<ITraderInfoGrain>(_trader_key);
        IStrategyGrain strategyGrain = newClusterClient.GetGrain<IStrategyGrain>(_strategy_key);

        return OnNewOrder(_orig_live_order, traderInfoGrain, strategyGrain);
    }

    public async override Task<int> EvaluateFilters(string trader_key, string strategy_key, 
        ITraderInfoGrain traderGrain, IStrategyGrain strategyGrain)
    {
        int includeExclude = 0;
        ScoreCardDTO _scoreCard = await traderGrain.GetScoreCardAsync(trader_key);    
        foreach (IStrategyFilter filter in _filters)
        {
            includeExclude = filter.IsInFilter(_scoreCard);
        }
        return includeExclude;
    }


    public async override Task ProcessOrderForStrategy(string strategy_key, NewOrderDTO order)
    {
        if(order.OrderAction != OrderAction.NoAction)
        {
            IOrderGrain orderGrain = newClusterClient.GetGrain<IOrderGrain>("T"+_strategyData.strategy_traderId + "-"+ orderCount);
            await orderGrain.ProcessOrder(order);
            orderCount++;
        }
    }
}
