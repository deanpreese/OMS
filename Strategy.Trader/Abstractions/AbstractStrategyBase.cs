using System.Security.Cryptography;
using Microsoft.Extensions.DependencyModel.Resolution;
using System.Threading.Tasks.Dataflow;

using Strategy.Trader.Abstractions;
using Strategy.Trader.Models;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.SharedKernel.Grains;
using OMS.SharedKernel;
using Strategy.Trader.Filters;

namespace Strategy.Trader.Abstractions;

public abstract class AbstractStrategyBase 
{
    public int orderCount = 1;

    public StrategyAccount _strategyData {get; set;}
    public string _strategy_key {get; set;}

    public List<IStrategyFilter> _filters = new List<IStrategyFilter>();

    public abstract Task<int> EvaluateFilters(string trader_key, string strategy_key, 
        ITraderInfoGrain traderGrain, IStrategyGrain strategyGrain);
   
    public abstract Task ProcessOrderForStrategy(string strategy_key, NewOrderDTO order);

    public abstract Task<NewOrderDTO> OnNewOrder(LiveOrderDTO _orig_live_order);

    public BufferBlock<string> flowBuffer;
   

    public async Task<NewOrderDTO> OnNewOrder(LiveOrderDTO _orig_live_order,
        ITraderInfoGrain traderGrain, IStrategyGrain strategyGrain)
    {
        //await AddToLogBuffer("  ----  ");
        //await AddToLogBuffer("New Order2222: " + _strategy_key + "  " + traderGrain.GetGrainId() + "  " + _orig_live_order.OrderAction + "  " + _orig_live_order.OrderType);

        LiveOrderDTO strategy_order = await GenerateOrderAction( _strategy_key, _orig_live_order, traderGrain, strategyGrain);
        NewOrderDTO _mapped_new_order = await SharedMapping.MapLiveOrderDTOLiveToNewDTO(_orig_live_order);
        
        if(strategy_order.OrderAction != OrderAction.NoAction)
        {
            _mapped_new_order = await SharedMapping.MapLiveOrderDTOLiveToNewDTO(strategy_order);
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

    public async Task<LiveOrderDTO> GenerateOrderAction(string strategy_key, LiveOrderDTO order, 
        ITraderInfoGrain _trader_grain, IStrategyGrain _strategy_grain)
    {
        string _trader_key = order.UserID + "_" + order.GroupID;
        //await AddToLogBuffer("New Order: " + _strategy_key + "  " + _trader_grain.GetGrainId() + "  " + order.OrderAction + "  " + order.OrderType);
        
        if (order.OrderType == OrderType.OPEN)
        {
            int filterAction = await EvaluateFilters(_trader_key, strategy_key, _trader_grain, _strategy_grain );
            //await AddToLogBuffer(_strategyData.strategy_name +  " __Filter Action Result: " + filterAction);
    
            if(!await OkToOpenNewPosition(_strategy_grain, order))
            {
                order.OrderAction = OrderAction.NoAction;
                await ShowStrategyInfo(order, OrderType.NONE); 
            }else
            {
                if (filterAction == 0)
                {
                    order.OrderAction = OrderAction.NoAction;
                    await ShowStrategyInfo(order, OrderType.NONE);
                }    

                // FilterAction > 0 means FOLLOW -- do the same 
                // Nothing Changes
                if (filterAction > 0 )
                {
                    await ShowStrategyInfo(order, OrderType.OPEN);
                }

                //  filterAction < 0 means FADE -- do the opposite
                // Reverse Current Order
                if (filterAction < 0)
                {
                    if (order.OrderAction == OrderAction.Buy)
                    {
                        order.OrderAction = OrderAction.Sell;
                        await ShowStrategyInfo(order, OrderType.OPEN);
                    }

                    if (order.OrderAction == OrderAction.Sell)
                    {
                        order.OrderAction = OrderAction.Buy;
                        await ShowStrategyInfo(order, OrderType.OPEN);
                    }
                }
            }
        }

        if (order.OrderType == OrderType.CLOSE)
        {
            order.OrderAction = OrderAction.NoAction;
            List<LiveOrderDTO> liveOrders = await _strategy_grain.GetLiveOrders(_strategy_key);
            
            if(liveOrders.Count == 0)
            {
                liveOrders = await _strategy_grain.GetLiveOrders(_strategy_key);
            }
            

            if(liveOrders.Count > 0)
            {
                await AddToLogBuffer(_strategyData.strategy_name +  " Strategy Orders Count: " + liveOrders.Count);
                LiveOrderDTO liveStrategyOrder = liveOrders.FirstOrDefault();
                ClosedTradeDTO lastClosedTraderTrade = await _trader_grain.GetLastClosedTradeByOpenPlatformID(_trader_key, liveStrategyOrder.RelatedOrderID);
                
                //while (lastClosedTraderTrade == null)
                //{
                //    lastClosedTraderTrade = await _trader_grain.GetLastClosedTradeByOpenPlatformID(_trader_key, liveStrategyOrder.PlatformOrderID);
                //    await AddToLogBuffer(_strategyData.strategy_name +  " Strategy Last Order: " + liveStrategyOrder.OrderAction + "  " + liveStrategyOrder.OrderType);
                //}

                if(liveStrategyOrder != null && lastClosedTraderTrade != null)
                {
                    if (liveStrategyOrder.OrderAction == OrderAction.Buy)
                    {
                        order.OrderAction = OrderAction.Sell;
                        await ShowStrategyInfo(order, OrderType.CLOSE);
                    }
                    if (liveStrategyOrder.OrderAction == OrderAction.Sell)
                    {
                        order.OrderAction = OrderAction.Buy;
                        await ShowStrategyInfo(order, OrderType.CLOSE);
                    }
                }
            }else
            {
                await ShowStrategyInfo(order, OrderType.NONE);
                await AddToLogBuffer(_strategyData.strategy_name +  " NULL ORDER: " + liveOrders.Count);
            }            
        }
        return order;
    }

    public async Task<bool> OkToOpenNewPosition(IStrategyGrain strategyGrain, LiveOrderDTO order)
    {   
        bool newPosition = true;
        
        List<LiveOrderDTO> liveOrders = await strategyGrain.GetLiveOrders(_strategy_key);

        for (int i = 0; i < 10; i++)
        {
            liveOrders = await strategyGrain.GetLiveOrders(_strategy_key);
        }

        //await AddToLogBuffer(_strategyData.strategy_name +  " Strategy Orders Count for New: " + liveOrders.Count);

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


    public async Task ShowStrategyInfo(LiveOrderDTO order, OrderType orderType)
    {
        string data = order.OrderTime + " " + _strategyData.strategy_name +  "  " + _strategy_key + "  " + order.OrderAction + "  " + orderType  + "  " + order.OrderType;
        await flowBuffer.SendAsync(data); 
    }

    public async Task AddToLogBuffer(string strData)
    {
        await flowBuffer.SendAsync(strData); 
    }

    public async Task ProcessLogBuffer()
    {
        
        while (await flowBuffer.OutputAvailableAsync()) 
        {
            string logData = flowBuffer.Receive();
            Console.WriteLine(logData);
        }
    }

    
}

