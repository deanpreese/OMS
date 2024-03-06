using OMS.Core.Common;
using OMS.Core.Models;
using Strategy.Trader.Abstractions;
using Strategy.Trader.Models;

namespace Strategy.Trader;

public class NStrategy : IStrategy
{
    public Task<NewOrder> OnNewOrder(LiveOrder order)
    {
        NewOrder newOrder = new NewOrder
        {
            Instrument = order.Instrument,
            OrderAction = OrderAction.NoAction

        };

        return Task.FromResult(newOrder);
    }

}
