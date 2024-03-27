using System.Security.Cryptography;
using Microsoft.Extensions.DependencyModel.Resolution;
using System.Threading.Tasks.Dataflow;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.SharedKernel;
using Strategy.Trader.Filters;
using Strategy.SharedKernel;

namespace Strategy.Trader.Abstractions;

public abstract class AbstractStrategy 
{
    // Strategy and Trader Properties
    public IStrategyConnection _strategyConnection;
    public List<IStrategyFilter> _filters = new List<IStrategyFilter>();

    public abstract Task<int> EvaluateFilters(ScoreCardDTO scoreCard);
    public abstract Task<NewOrderDTO> OnNewData(LiveOrderDTO traderLiveOrderDTO);


    public async Task OnTraderModelData(ModelOrderLogDTO modelOrderLogDTO)
    {
        await _strategyConnection.OnTraderModelData(modelOrderLogDTO);
        await OnNewData(modelOrderLogDTO.LiveOrderDeserialized);
    }

    public Task<int> AddFilter(IStrategyFilter filter)
    {
        _filters.Add(filter);
        return Task.FromResult(0);
    }

    public async Task<NewOrderDTO> ProcessNewData(LiveOrderDTO traderLiveOrder)
    {
        LiveOrderDTO _orig_trader_order = traderLiveOrder;

        LiveOrderDTO strategy_order = await GenerateOrderAction(_orig_trader_order);
        NewOrderDTO _mapped_new_order = await SharedMapping.MapLiveOrderDTOLiveToNewDTO(_orig_trader_order);
        
        if(strategy_order.OrderAction != OrderAction.NoAction)
        {
            _mapped_new_order = await SharedMapping.MapLiveOrderDTOLiveToNewDTO(strategy_order);
            _mapped_new_order.GroupID = _strategyConnection.GetStrategyAccount().Result.group;
            _mapped_new_order.UserID = _strategyConnection.GetStrategyAccount().Result.strategy_traderId;
            _mapped_new_order.Quantity = strategy_order.Quantity;
            _mapped_new_order.RelatedOrderID = _orig_trader_order.PlatformOrderID;

            if(_mapped_new_order.OrderAction != OrderAction.NoAction)
            {
                await _strategyConnection.ProcessOrderForStrategy( _mapped_new_order);
            }

        }else
        {
            _mapped_new_order.OrderAction = OrderAction.NoAction;
        }
        
        return _mapped_new_order;
    }

    public async Task<LiveOrderDTO> GenerateOrderAction(LiveOrderDTO origTraderLiveOrder)
    {
       
        if (origTraderLiveOrder.OrderType == OrderType.OPEN)
        {
            int filterAction = await EvaluateFilters(await _strategyConnection.GetTraderScoreCard());
            //await AddToLogBuffer(_strategyData.strategy_name +  " __Filter Action Result: " + filterAction);
    
            if(!await OkToOpenNewPosition(origTraderLiveOrder))
            {
                origTraderLiveOrder.OrderAction = OrderAction.NoAction;
                await LogStrategyData(origTraderLiveOrder, OrderType.NONE); 
            }else
            {
                if (filterAction == 0)
                {
                    origTraderLiveOrder.OrderAction = OrderAction.NoAction;
                    await LogStrategyData(origTraderLiveOrder, OrderType.NONE);
                }    

                // FilterAction > 0 means FOLLOW -- do the same 
                // Nothing Changes
                if (filterAction > 0 )
                {
                    await LogStrategyData(origTraderLiveOrder, OrderType.OPEN);
                }

                //  filterAction < 0 means FADE -- do the opposite
                // Reverse Current Order
                if (filterAction < 0)
                {
                    if (origTraderLiveOrder.OrderAction == OrderAction.Buy)
                    {
                        origTraderLiveOrder.OrderAction = OrderAction.Sell;
                        await LogStrategyData(origTraderLiveOrder, OrderType.OPEN);
                    }

                    if (origTraderLiveOrder.OrderAction == OrderAction.Sell)
                    {
                        origTraderLiveOrder.OrderAction = OrderAction.Buy;
                        await LogStrategyData(origTraderLiveOrder, OrderType.OPEN);
                    }
                }
            }
        }

        if (origTraderLiveOrder.OrderType == OrderType.CLOSE)
        {
            origTraderLiveOrder.OrderAction = OrderAction.NoAction;
            List<LiveOrderDTO> liveOrders = await _strategyConnection.GetStrategyLiveOrders();

            if(liveOrders.Count > 0)
            {
                LiveOrderDTO liveStrategyOrder = liveOrders.FirstOrDefault();
                ClosedTradeDTO lastClosedTraderTrade = await _strategyConnection.GetLastClosedTraderTradeByOpenPlatformID(liveStrategyOrder.RelatedOrderID);

                if(liveStrategyOrder != null && lastClosedTraderTrade != null)
                {
                    if (liveStrategyOrder.OrderAction == OrderAction.Buy)
                    {
                        origTraderLiveOrder.OrderAction = OrderAction.Sell;
                        await LogStrategyData(origTraderLiveOrder, OrderType.CLOSE);
                    }
                    if (liveStrategyOrder.OrderAction == OrderAction.Sell)
                    {
                        origTraderLiveOrder.OrderAction = OrderAction.Buy;
                        await LogStrategyData(origTraderLiveOrder, OrderType.CLOSE);
                    }
                }
            }else
            {
                await LogStrategyData(origTraderLiveOrder, OrderType.NONE);
            }            
        }
        return origTraderLiveOrder;
    }

    public async Task<bool> OkToOpenNewPosition(LiveOrderDTO origTraderLiveOrder)
    {   
        bool newPosition = true;
        
        List<LiveOrderDTO> liveOrders = await _strategyConnection.GetStrategyLiveOrders();

        for (int i = 0; i < 10; i++)
        {
            liveOrders = await _strategyConnection.GetStrategyLiveOrders();
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

    public async Task LogStrategyData(LiveOrderDTO liveOrderDTO, OrderType orderType)
    {
        string logData = "  ";  
        await Task.Run(() =>
        {
            Console.WriteLine(logData);
        });           
    }

    
}

