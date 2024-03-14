using System.Security.Cryptography;
using Microsoft.Extensions.DependencyModel.Resolution;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Core.Models;
using Strategy.Trader.Abstractions;
using Strategy.Trader.Models;

namespace Strategy.Trader.Abstractions;

public abstract class AbstractStrategyBase 
{
    public StrategyAccount _strategyData {get; set;}
    public string _strategy_key {get; set;}

    public List<IStrategyFilter> _filters = new List<IStrategyFilter>();

    public abstract Task<int> EvaluateFilters(string trader_key, string strategy_key, 
        ITraderInfoGrain traderGrain, IStrategyGrain strategyGrain);
   
    public abstract Task ProcessOrderForStrategy(string strategy_key, NewOrder order);

    public abstract Task<NewOrder> OnNewOrder(LiveOrder _orig_live_order);

    public async Task<NewOrder> OnNewOrder(LiveOrder _orig_live_order,
        ITraderInfoGrain traderGrain, IStrategyGrain strategyGrain)
    {
        Console.WriteLine("  ----  ");
        //Console.WriteLine("New Order2222: " + _strategy_key + "  " + traderGrain.GetGrainId() + "  " + _orig_live_order.OrderAction + "  " + _orig_live_order.OrderType);

        LiveOrder strategy_order = await GenerateOrderAction( _strategy_key, _orig_live_order, traderGrain, strategyGrain);
        NewOrder _mapped_new_order = await OrderMapping.MapOrderLiveToNew(_orig_live_order);

        
        if(strategy_order.OrderAction != OrderAction.NoAction)
        {
            _mapped_new_order = await OrderMapping.MapOrderLiveToNew(strategy_order);
            _mapped_new_order.GroupID = _strategyData.group;
            _mapped_new_order.UserID = _strategyData.strategy_traderId;
            _mapped_new_order.Quantity =strategy_order.Quantity;
            _mapped_new_order.RelatedOrderID = _orig_live_order.PlatformOrderID;

            if(_mapped_new_order.OrderAction != OrderAction.NoAction)
            {
                await ProcessOrderForStrategy(_strategy_key, _mapped_new_order);
            }

        }else
        {
            _mapped_new_order.OrderAction = OrderAction.NoAction;
        }
        
        return _mapped_new_order;
    }

    public async Task<LiveOrder> GenerateOrderAction(string strategy_key, LiveOrder order, 
        ITraderInfoGrain _trader_grain, IStrategyGrain _strategy_grain)
    {
        string _trader_key = order.UserID + "_" + order.GroupID;
        //Console.WriteLine("New Order3333: " + _strategy_key + "  " + _trader_grain.GetGrainId() + "  " + order.OrderAction + "  " + order.OrderType);
        
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

                //while (lastClosedTraderTrade == null)
                //{
                //    lastClosedTraderTrade = await _trader_grain.GetLastClosedTradeByOpenPlatformID(_trader_key, liveStrategyOrder.PlatformOrderID);
                //    Console.WriteLine(_strategyData.strategy_name +  " Strategy Last Order: " + liveStrategyOrder.OrderAction + "  " + liveStrategyOrder.OrderType);
                //}

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

    public async Task<bool> OkToOpenNewPosition(IStrategyGrain strategyGrain, LiveOrder order)
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


    public async Task ShowOrderInfo(LiveOrder order, OrderType orderType)
    {
        await Task.Run(() =>
        {
            Console.WriteLine(_strategyData.strategy_name +  " Strategy Order: " + _strategy_key + "  " + order.OrderAction + "  " + orderType  + "  " + order.OrderType);
        });           
    }


    
}

