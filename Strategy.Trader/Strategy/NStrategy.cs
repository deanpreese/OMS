using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Core.Models;
using Strategy.Trader.Abstractions;
using Strategy.Trader.Models;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Strategy.Trader;

public class NStrategy : AbstractStrategyBase, IStrategy
{
    IClusterClient newClusterClient;

    public NStrategy() { }

    public NStrategy(IClusterClient clusterClient, StrategyAccount strategyData) 
    {
        newClusterClient = clusterClient;        
        _strategyData = strategyData;
    }


    public override Task<int> EvaluateFilters(string trader_key, string strategy_key, ITraderInfoGrain traderGrain, IStrategyGrain strategyGrain)
    {
        return Task.FromResult(0);
    }

    public override Task<NewOrderDTO> OnNewOrder(LiveOrder order)
    {
        NewOrderDTO newOrder = new NewOrderDTO
        {
            Instrument = order.Instrument,
            OrderAction = OrderAction.NoAction

        };
        

        return Task.FromResult(newOrder);
    }


    public override Task ProcessOrderForStrategy(string strategy_key, NewOrderDTO order)
    {
        throw new NotImplementedException();
    }
}
