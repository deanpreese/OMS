
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyModel.Resolution;
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
        Console.WriteLine("           ----  ");

        LiveOrder strategy_order = await GenerateOrderAction( _strategy_key, _orig_live_order);
        NewOrder _mapped_new_order = OrderMapping.MapOrderLiveToNew(_orig_live_order);

        if(strategy_order.OrderAction != OrderAction.NoAction)
        {
            _mapped_new_order = OrderMapping.MapOrderLiveToNew(strategy_order);
            _mapped_new_order.GroupID = _strategyData.group;
            _mapped_new_order.UserID = _strategyData.strategy_traderId;
            _mapped_new_order.Quantity =strategy_order.Quantity;
            _mapped_new_order.RelatedOrderID = _orig_live_order.PlatformOrderID;
        }else
        {
            _mapped_new_order.OrderAction = OrderAction.NoAction;
        }

        return _mapped_new_order;

    }

    public async Task<LiveOrder> GenerateOrderAction(string strategy_key, LiveOrder order)
    {

        string _trader_key = order.UserID + "_" + order.GroupID;
        Console.WriteLine(_strategyData.strategy_name + " New Order " + _trader_key + "  " + order.OrderAction + "  " + order.OrderType) ;

        ITraderGrain _trader_grain = _grainFactory.GetGrain<ITraderGrain>(_trader_key);
        IStrategyGrain  _strategy_grain = _grainFactory.GetGrain<IStrategyGrain>(_strategy_key);

        if (order.OrderType == OrderType.OPEN)
        {
            int filterAction = await EvaluateFilters(_trader_key, strategy_key, _trader_grain, _strategy_grain );
            Console.WriteLine(_strategyData.strategy_name +  " __Filter Action Result: " + filterAction);
    
            if(!await OkToOpenNewPosition(_strategy_grain, order))
            {
                order.OrderAction = OrderAction.NoAction;
                await ShowOrderInfo(order, OrderType.NONE); 
            }else
            {
                if (filterAction == 0)
                {
                    order.OrderAction = OrderAction.NoAction;
                    await ShowOrderInfo(order, OrderType.NONE);
                }    

                // FilterAction > 0 means FOLLOW -- do the same 
                // Nothing Changes
                if (filterAction > 0 )
                {
                    await ShowOrderInfo(order, OrderType.OPEN);
                }

                //  filterAction < 0 means FADE -- do the opposite
                // Reverse Current Order
                if (filterAction < 0)
                {
                    if (order.OrderAction == OrderAction.Buy)
                    {
                        order.OrderAction = OrderAction.Sell;
                        await ShowOrderInfo(order, OrderType.OPEN);
                    }

                    if (order.OrderAction == OrderAction.Sell)
                    {
                        order.OrderAction = OrderAction.Buy;
                        await ShowOrderInfo(order, OrderType.OPEN);
                    }
                }
            }

        }

        if (order.OrderType == OrderType.CLOSE)
        {
            order.OrderAction = OrderAction.NoAction;
            List<LiveOrder> liveOrders = await _strategy_grain.GetLiveOrders(_strategy_key);

            if(liveOrders.Count == 0)
            {
                liveOrders = await _strategy_grain.GetLiveOrders(_strategy_key);
            }

            Console.WriteLine(_strategyData.strategy_name +  " Strategy Orders Count: " + liveOrders.Count);

            if(liveOrders.Count > 0)
            {
                LiveOrder liveStrategyOrder = liveOrders.FirstOrDefault();
                ClosedTrade lastClosedTraderTrade = await _trader_grain.GetLastClosedTradeByOpenPlatformID(_trader_key, liveStrategyOrder.RelatedOrderID);

                while (lastClosedTraderTrade == null)
                {
                    lastClosedTraderTrade = await _trader_grain.GetLastClosedTradeByOpenPlatformID(_trader_key, liveStrategyOrder.PlatformOrderID);
                    Console.WriteLine(_strategyData.strategy_name +  " Strategy Last Order: " + liveStrategyOrder.OrderAction + "  " + liveStrategyOrder.OrderType);
                }

                if(liveStrategyOrder != null && lastClosedTraderTrade != null)
                {
                    if (liveStrategyOrder.OrderAction == OrderAction.Buy)
                    {
                        order.OrderAction = OrderAction.Sell;
                        await ShowOrderInfo(order, OrderType.CLOSE);
                    }
                    if (liveStrategyOrder.OrderAction == OrderAction.Sell)
                    {
                        order.OrderAction = OrderAction.Buy;
                        await ShowOrderInfo(order, OrderType.CLOSE);
                    }
                }
            }else
            {
                await ShowOrderInfo(order, OrderType.NONE);
                Console.WriteLine(_strategyData.strategy_name +  " NULL ORDER: " + liveOrders.Count);
            }            
        }

        return order;
    }

    private async Task<bool> OkToOpenNewPosition(IStrategyGrain strategyGrain, LiveOrder order)
    {   
        bool newPosition = true;
        
        List<LiveOrder> liveOrders = await strategyGrain.GetLiveOrders(_strategy_key);

        for (int i = 0; i < 10; i++)
        {
            liveOrders = await strategyGrain.GetLiveOrders(_strategy_key);
        }

        Console.WriteLine(_strategyData.strategy_name +  " Strategy Orders Count for New: " + liveOrders.Count);

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


    private async Task ShowOrderInfo(LiveOrder order, OrderType orderType)
    {
        await Task.Run(() =>
        {
            Console.WriteLine(_strategyData.strategy_name +  " Strategy Order: " + _strategy_key + "  " + order.OrderAction + "  " + orderType  + "  " + order.OrderType);
        });           
    }


    
}

