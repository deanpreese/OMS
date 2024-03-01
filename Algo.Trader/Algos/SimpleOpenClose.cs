using Algo.Trader.Abstractions;
using Algo.Trader.Filters;
using Algo.Trader.Models;
using OMS.Core.Common;
using OMS.Core.Models;

using OMS.Grains.Interfaces;

namespace Algo.Trader.Trader;

public class SimpleOpenClose : IAlgo 
{
     public IGrainFactory _grainFactory;
     public AlgoData _algoData;
     private string _algo_key;
      public List<IAlgoFilter> _filters = new List<IAlgoFilter>();

    public SimpleOpenClose(IGrainFactory grainFactory, AlgoData algoData) 
    {
        _grainFactory = grainFactory;
        _algoData = algoData;
        _algo_key = _algoData.algo_traderId + "_" + _algoData.group;
                
    }

    public async Task<NewOrder> GenerateAlgoOrder(LiveOrder _orig_live_order)
    {
        Console.WriteLine("   ");

        NewOrder _mapped_new_order = OrderMapping.MapOrderLiveToNew(_orig_live_order);
        
        string _trader_key = _orig_live_order.UserID + "_" + _orig_live_order.GroupID;
        Console.WriteLine("--- > Order From : " + _trader_key + "  " + _orig_live_order.OrderAction ) ;                       

        ITraderGrain _trader_grain = _grainFactory.GetGrain<ITraderGrain>(_trader_key);
        IAlgoGrain  _algo_grain = _grainFactory.GetGrain<IAlgoGrain>(_algo_key);

        _filters.Add(new NewAlgoFilter(await _trader_grain.GetProfileAsync(_trader_key), await _trader_grain.GetScoreCardAsync(_trader_key)));

        LiveOrder algo_order = await DetermineAlgoAction(_trader_key, _algo_key,  _trader_grain, _algo_grain, _orig_live_order);
        _mapped_new_order = OrderMapping.MapOrderLiveToNew(algo_order);
        _mapped_new_order.GroupID = _algoData.group;
        _mapped_new_order.UserID = _algoData.algo_traderId;
        _mapped_new_order.RelatedOrderID = _orig_live_order.PlatformOrderID;        

        Console.WriteLine("   ");

        return _mapped_new_order;

    }

    public async Task<LiveOrder> DetermineAlgoAction(string trader_key, string algo_key, ITraderGrain _traderGrain, IAlgoGrain _algoGrain, LiveOrder order)
    {
        int filterAction = CheckFilters();

        List<LiveOrder> traderOrders = await _traderGrain.GetLiveOrders(trader_key);        
        int _traderOpenOrdersCount = traderOrders.Count;

        Console.WriteLine($"_x_Trader {trader_key} open orders:" + _traderOpenOrdersCount);
        Console.WriteLine("__Filter Action: " + filterAction);

        try {

            // must be new opening order
            // Gen new order based on rules
            if (_traderOpenOrdersCount > 0)
            {
                // FilterAction > 0 means FOLLOW -- do the same 
                // Nothing Changes
                if (filterAction > 0 )
                {
                    PrintOrderInfo(order, OrderType.OPEN);
                }

                //  filterAction < 0 means FADE -- do the opposite
                // Reverse Current Order
                if (filterAction < 0)
                {
                    if (order.OrderAction == OrderAction.Buy)
                    {
                        order.OrderAction = OrderAction.Sell;
                        PrintOrderInfo(order, OrderType.OPEN);
                    }

                    if (order.OrderAction == OrderAction.Sell)
                    {
                        order.OrderAction = OrderAction.Buy;
                        PrintOrderInfo(order, OrderType.OPEN);
                    }
                }
            }

            // must be new closing order
            // Get the trade and figure out the action
            if (_traderOpenOrdersCount == 0)
                {
                    ClosedTrade closed = await _traderGrain.GetLastClosedTrade(trader_key);
                    List<LiveOrder> liveOrders = await _algoGrain.GetLiveOrders(_algo_key);

                    if(liveOrders.Any())

                    {
                        LiveOrder lastOrder = liveOrders.Find(x => x.RelatedOrderID == closed.OpenPlatformOrderID);

                        if(lastOrder != null)
                        {
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
                    }
            }
        }catch (Exception ex)
        {
            Console.WriteLine("DetermineAlgoAction ERROR " + ex.StackTrace);
        }


        return order;
    }

    public int CheckFilters()
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
        Console.WriteLine("New Algo Order: " + _algo_key + "  " + order.OrderAction + "  " + orderType);
    }

}
