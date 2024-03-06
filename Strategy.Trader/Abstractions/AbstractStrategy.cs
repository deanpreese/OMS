
using System.Security.Cryptography;
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
        _filters = AddFilters();                
    }

    public abstract Task<int> EvaluateFilters(string trader_key, string strategy_key,ITraderGrain traderGrain, IStrategyGrain strategyGrain);
    public abstract List<IStrategyFilter>  AddFilters();
    

    public async Task<NewOrder> OnNewOrder(LiveOrder _orig_live_order)
    {
        LiveOrder strategy_order = await GenerateOrderAction( _strategy_key, _orig_live_order);
        NewOrder _mapped_new_order = OrderMapping.MapOrderLiveToNew(_orig_live_order);

        if(strategy_order.OrderAction != OrderAction.NoAction)
        {
            _mapped_new_order = OrderMapping.MapOrderLiveToNew(strategy_order);
            _mapped_new_order.GroupID = _strategyData.group;
            _mapped_new_order.UserID = _strategyData.strategy_traderId;
            _mapped_new_order.Quantity =strategy_order.Quantity;
            _mapped_new_order.RelatedOrderID = _orig_live_order.PlatformOrderID;
            IOrderGrain orderGrain = _grainFactory.GetGrain<IOrderGrain>(_strategy_key);     
            await orderGrain.ProcessOrder(_mapped_new_order);
        }else
        {
            _mapped_new_order.OrderAction = OrderAction.NoAction;
        }

        return _mapped_new_order;

    }

    public async Task<LiveOrder> GenerateOrderAction(string strategy_key, LiveOrder order)
    {
        string _trader_key = order.UserID + "_" + order.GroupID;
        Console.WriteLine("---> Order From : " + _trader_key + "  " + order.OrderAction ) ;

        ITraderGrain _trader_grain = _grainFactory.GetGrain<ITraderGrain>(_trader_key);
        IStrategyGrain  _strategy_grain = _grainFactory.GetGrain<IStrategyGrain>(_strategy_key);

        List<LiveOrder> traderOrders = await _trader_grain.GetLiveOrders(_trader_key);        
        int _traderOpenOrdersCount = traderOrders.Count;
        Console.WriteLine($"_Trader {_trader_key} open orders: " + _traderOpenOrdersCount);

        try {
            // must be new opening order
            // Gen new order based on rules

            if (_traderOpenOrdersCount > 0)
            {
                int filterAction = await EvaluateFilters(_trader_key, strategy_key, _trader_grain, _strategy_grain );
                Console.WriteLine("__Filter Action Result: " + filterAction);

                
                if(!await OpenNewPosition(_strategy_grain, order))
                {
                    order.OrderAction = OrderAction.NoAction;
                    ShowOrderInfo(order, OrderType.NONE); 
                    return order;                   
                }
               

                if (filterAction == 0)
                {
                    order.OrderAction = OrderAction.NoAction;
                    ShowOrderInfo(order, OrderType.NONE);
                }    

                // FilterAction > 0 means FOLLOW -- do the same 
                // Nothing Changes
                if (filterAction > 0 )
                {
                    ShowOrderInfo(order, OrderType.OPEN);
                }

                //  filterAction < 0 means FADE -- do the opposite
                // Reverse Current Order
                if (filterAction < 0)
                {
                    if (order.OrderAction == OrderAction.Buy)
                    {
                        order.OrderAction = OrderAction.Sell;
                        ShowOrderInfo(order, OrderType.OPEN);
                    }

                    if (order.OrderAction == OrderAction.Sell)
                    {
                        order.OrderAction = OrderAction.Buy;
                        ShowOrderInfo(order, OrderType.OPEN);
                    }
                }
            }

            // must be new closing order
            // Get the trade and figure out the action
            if (_traderOpenOrdersCount == 0)
                {
                    Console.WriteLine("Processing Closing Order");
                    order.OrderAction = OrderAction.NoAction;

                    ClosedTrade closed = await _trader_grain.GetLastClosedTrade(_trader_key);
                    List<LiveOrder> liveOrders = await _strategy_grain.GetLiveOrders(_strategy_key);

                    Console.WriteLine("Closed Trade ID : " + closed.StorerID); 
                    Console.WriteLine("Strategy Orders Count: " + liveOrders.Count);

                    if(liveOrders.Any())
                    {
                        LiveOrder lastOrder = liveOrders.Find(x => x.RelatedOrderID == closed.OpenPlatformOrderID);

                        if(lastOrder != null)
                        {
                            if (lastOrder.OrderAction == OrderAction.Buy)
                            {
                                order.OrderAction = OrderAction.Sell;
                                ShowOrderInfo(order, OrderType.CLOSE);
                            }
                            if (lastOrder.OrderAction == OrderAction.Sell)
                            {
                                order.OrderAction = OrderAction.Buy;
                                ShowOrderInfo(order, OrderType.CLOSE);
                            }
                        }
                    }else
                    {
                        ShowOrderInfo(order, OrderType.NONE);
                    }

            }
        }catch (Exception ex)
        {
            Console.WriteLine("DetermineAlgoAction ERROR " + ex.StackTrace);
        }


        return order;
    }

    private async Task<bool> OpenNewPosition(IStrategyGrain strategyGrain, LiveOrder order)
    {   
        bool newPosition = true;
        List<LiveOrder> liveOrders = await strategyGrain.GetLiveOrders(_strategy_key);
        if(liveOrders.Count > 0)
        {
            //int ordersSameDirection = liveOrders.FindAll(x => x.OrderAction == order.OrderAction).Count();
            //if (_strategyData.orders_per_direction >= ordersSameDirection)
            //{
                newPosition = false;        
            //}
        }
        return  newPosition;
    }


    private void ShowOrderInfo(LiveOrder order, OrderType orderType)
    {
        Console.WriteLine("New Strategy Order: " + _strategy_key + "  " + order.OrderAction + "  " + orderType);
    }

    
}

