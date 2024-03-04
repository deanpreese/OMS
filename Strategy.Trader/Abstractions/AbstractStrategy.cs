
using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Core.Models;


using Strategy.Trader.Abstractions;
using Strategy.Trader.Models;


namespace Strategy.Trader.Abstractions;

public abstract class AbstractStrategy : IStrategy 
{
     public IGrainFactory _grainFactory;
     public StrategyAccount _strategyData;
     private string _strategy_key;
      public List<IStrategyFilter> _filters = new List<IStrategyFilter>();

    public AbstractStrategy(IGrainFactory grainFactory, StrategyAccount strategyData) 
    {
        _grainFactory = grainFactory;
        _strategyData = strategyData;
        _strategy_key = _strategyData.strategy_traderId + "_" + _strategyData.group;
                
    }

    public abstract int CheckFilters();
    public abstract List<IStrategyFilter>  AddOrderFilters(UserProfile userProfile, ScoreCard scoreCard);
    

    public async Task<NewOrder> GenerateAlgoOrder(LiveOrder _orig_live_order)
    {
        Console.WriteLine("   ");

        NewOrder _mapped_new_order = OrderMapping.MapOrderLiveToNew(_orig_live_order);
        
        string _trader_key = _orig_live_order.UserID + "_" + _orig_live_order.GroupID;
        Console.WriteLine("--- > Order From : " + _trader_key + "  " + _orig_live_order.OrderAction ) ;                       

        ITraderGrain _trader_grain = _grainFactory.GetGrain<ITraderGrain>(_trader_key);
        IStrategyGrain  _strategy_grain = _grainFactory.GetGrain<IStrategyGrain>(_strategy_key);

        _filters = AddOrderFilters(await _trader_grain.GetProfileAsync(_trader_key), await _trader_grain.GetScoreCardAsync(_trader_key));
        LiveOrder algo_order = await DetermineAlgoAction(_trader_key, _strategy_key,  _trader_grain, _strategy_grain, _orig_live_order);

        _mapped_new_order = OrderMapping.MapOrderLiveToNew(algo_order);
        _mapped_new_order.GroupID = _strategyData.group;
        _mapped_new_order.UserID = _strategyData.strategy_traderId;
        _mapped_new_order.RelatedOrderID = _orig_live_order.PlatformOrderID;        

        Console.WriteLine("   ");

        return _mapped_new_order;

    }

    public async Task<LiveOrder> DetermineAlgoAction(string trader_key, string algo_key, ITraderGrain _traderGrain, IStrategyGrain _algoGrain, LiveOrder order)
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
                Console.WriteLine("Processing Opening Order");

                if (filterAction == 0)
                {
                    order.OrderAction = OrderAction.NoAction;
                    PrintOrderInfo(order, OrderType.NONE);
                }    

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
                    Console.WriteLine("Processing Closing Order");
                    order.OrderAction = OrderAction.NoAction;

                    ClosedTrade closed = await _traderGrain.GetLastClosedTrade(trader_key);
                    List<LiveOrder> liveOrders = await _algoGrain.GetLiveOrders(_strategy_key);

                    Console.WriteLine("Closed Trade ID : " + closed.StorerID); 
                    Console.WriteLine("Live Orders Count: " + liveOrders.Count);

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
                    }else
                    {
                        PrintOrderInfo(order, OrderType.NONE);
                    }

            }
        }catch (Exception ex)
        {
            Console.WriteLine("DetermineAlgoAction ERROR " + ex.StackTrace);
        }


        return order;
    }

    private void PrintOrderInfo(LiveOrder order, OrderType orderType)
    {
        Console.WriteLine("New Algo Order: " + _strategy_key + "  " + order.OrderAction + "  " + orderType);
    }

    
}

