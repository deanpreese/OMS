using Algo.Trader.Abstractions;
using Algo.Trader.Filters;
using Algo.Trader.Models;
using OMS.Core.Common;
using OMS.Core.Models;

namespace Algo.Trader.Trader;

public class SimpleOpenClose : AbstractBase, IAlgo 
{

    public SimpleOpenClose(IGrainFactory grainFactory, AlgoData algoData) : base(grainFactory, algoData)
    {
        _filters.Add(new NewAlgoFilter(_traderUserProfile, _traderScoreCard));
    }

    public NewOrder GenerateAlgoOrder(LiveOrder order)
    {
        trader_key = order.UserID + "_" + order.UserGroup;
        InitializeGrains(trader_key); 
       


        NewOrder n_o = OrderMapping.MapOrder(order);
        n_o.UserGroup = _algoData.group;
        n_o.UserID = _algoData.algo_traderId;
        n_o.RelatedOrderID = order.PlatformOrderID;        
        return n_o;

    }

    public override LiveOrder DetermineAlgoAction(LiveOrder order)
    {
        int filterAction = CheckFilters();

        // must be new opening order
        // Gen new order based on rules
        if (_traderOpenOrdersCount > 0)
        {
            if (filterAction > 0)
            {
                order.OrderAction = OrderAction.Buy;
                PrintOrderInfo(order, OrderType.OPEN);
            }
            if (filterAction < 0)
            {
                order.OrderAction = OrderAction.Sell;
                PrintOrderInfo(order, OrderType.OPEN);
            }
        }

        // must be new closing order
        // Get the trade and figure out the action
        if (_traderOpenOrdersCount == 0)
        {
            ClosedTrade closed = _traderLastClosedTrade;
            LiveOrder lastOrder = AlgoLiveOrders.Find(x => x.PlatformOrderID == closed.OpenPlatformOrderID);

            if (lastOrder.OrderAction == OrderAction.Buy)
            {
                order.OrderAction = OrderAction.Sell;
                PrintOrderInfo(order, OrderType.CLOSE);
            }
            if (lastOrder.OrderAction == OrderAction.Sell)
            {
                order.OrderAction = OrderAction.Buy;
                PrintOrderInfo(order, OrderType.CLOSE);
            }
        }


        return order;
    }

    public override int CheckFilters()
    {
        int includeExclude = 0;
        foreach (IAlgoFilter filter in _filters)
        {
            includeExclude = filter.IsInAlgoFilter();
        }
        return includeExclude;
    }
    
    private void PrintOrderInfo(LiveOrder order, OrderType orderType)
    {
        Console.WriteLine("Algo Order: " + order.UserID + "  " + order.UserGroup + "  " + order.OrderAction + "  " + orderType);
    }

}
