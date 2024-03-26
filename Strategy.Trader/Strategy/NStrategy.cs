
using Strategy.Trader.Abstractions;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Strategy.Trader;

public class NStrategy : AbstractStrategy, IStrategy
{

    public NStrategy() { }

    public NStrategy(IStrategyConnection strategyConnection) 
    {
    }

    public override Task<int> EvaluateFilters(ScoreCardDTO scoreCard)
    {
        return Task.FromResult(0);
    }

    public override Task<NewOrderDTO> OnNewData(LiveOrderDTO traderLiveOrderDTO)
    {
         NewOrderDTO newOrder = new NewOrderDTO
        {
            Instrument = "NONE",
            OrderAction = OrderAction.NoAction

        };

        // in a typical strategy would call 
        // newOrder = ProcessData();

        return Task.FromResult(newOrder);
    }

}
