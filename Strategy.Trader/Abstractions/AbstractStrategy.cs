using System.Security.Cryptography;
using Microsoft.Extensions.DependencyModel.Resolution;
using System.Threading.Tasks.Dataflow;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.SharedKernel;
using Strategy.Trader.Filters;
using Strategy.SharedKernel;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;


namespace Strategy.Trader.Abstractions;

public abstract class AbstractStrategy  :  ScreenColorBase
{

    // Strategy and Trader Properties
    public IStrategyConnection _strategyConnection;
    public StrategyAccount CurrentStrategyAccount { get; set; }
    public List<IStrategyFilter> _filters = new List<IStrategyFilter>();

    public abstract Task<int> EvaluateFilters(ScoreCardDTO scoreCard);
    public abstract Task<NewOrderDTO> OnNewData(LiveOrderDTO traderLiveOrderDTO);
    public int orderCount;

    public AbstractStrategy(IStrategyConnection strategyConnection)
    {
        _strategyConnection = strategyConnection;
        CurrentStrategyAccount = strategyConnection.CurrentStrategyAccount;
    }

    public async Task OnTraderModelData(ModelOrderLogDTO modelOrderLogDTO)
    {
        await Task.Run(async () =>
        {
            orderCount++;    
            await _strategyConnection.OnTraderModelData(modelOrderLogDTO);            
            await OnNewData(_strategyConnection.ModelTraderLiveOrderDTO);
        });
    }


    public Task<int> AddFilter(IStrategyFilter filter)
    {
        _filters.Add(filter);
        return Task.FromResult(0);
    }

    public async Task<NewOrderDTO> ProcessNewData(LiveOrderDTO traderLiveOrder)
    {
        LiveOrderDTO _orig_trader_order = traderLiveOrder;
        NewOrderDTO _mapped_new_order = await SharedMapping.MapLiveOrderDTOLiveToNewDTO(_orig_trader_order);
        
        LiveOrderDTO strategy_order = await GenerateOrderAction(_orig_trader_order);
        if(strategy_order.OrderAction != OrderAction.NoAction)
        {
            _mapped_new_order = await SharedMapping.MapLiveOrderDTOLiveToNewDTO(strategy_order);
            _mapped_new_order.GroupID = CurrentStrategyAccount.group;
            _mapped_new_order.UserID = CurrentStrategyAccount.strategy_traderId;
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
        
        Console.WriteLine(CurrentStrategyAccount.logid + "  " + orderCount);

        return _mapped_new_order;
    }

    public async Task<LiveOrderDTO> GenerateOrderAction(LiveOrderDTO traderOrder)
    {

        if (traderOrder.OrderType == OrderType.OPEN)
        {
            return await OpenOrderAction(traderOrder);
        }

        if (traderOrder.OrderType == OrderType.CLOSE)
        {
            return await CloseOrderAction(traderOrder);                   
        }

        return traderOrder;        

    }

    private async Task<LiveOrderDTO> CloseOrderAction(LiveOrderDTO traderOrder)
    {
        string _trader_key = traderOrder.UserID + "_" + traderOrder.GroupID;
        traderOrder.OrderAction = OrderAction.NoAction;
        List<LiveOrderDTO> liveOrders = await _strategyConnection.GetStrategyLiveOrders();

        //Console.WriteLine(CurrentStrategyAccount.strategy_name +  " Strategy Orders Count: " + liveOrders.Count);

        if(liveOrders.Count > 0)
        {
            LiveOrderDTO liveStrategyOrder = liveOrders.FirstOrDefault();
            //Console.WriteLine(CurrentStrategyAccount.strategy_name  +  " XREF: " + liveStrategyOrder.RelatedOrderID);
            ClosedTradeDTO lastClosedTraderTrade = await _strategyConnection.GetLastClosedTraderTradeByOpenPlatformID(_trader_key, liveStrategyOrder.RelatedOrderID);

            if(liveStrategyOrder != null && lastClosedTraderTrade != null)
            {
                if (liveStrategyOrder.OrderAction == OrderAction.Buy)
                {
                    traderOrder.OrderAction = OrderAction.Sell;
                    await LogStrategyData(_trader_key, traderOrder.OrderAction, traderOrder.OrderType); 
                    return traderOrder;
                }
                if (liveStrategyOrder.OrderAction == OrderAction.Sell)
                {
                    traderOrder.OrderAction = OrderAction.Buy;
                    await LogStrategyData(_trader_key, traderOrder.OrderAction, traderOrder.OrderType);
                    return traderOrder;
                }
            }
        }else
        {
            await LogStrategyData(_trader_key, traderOrder.OrderAction, OrderType.NONE); 
            Console.WriteLine(CurrentStrategyAccount.strategy_name +  " NULL ORDER: " + liveOrders.Count);
        }
        return traderOrder;       
    }



    private async Task<LiveOrderDTO> OpenOrderAction(LiveOrderDTO traderOrder)
    {
        string _trader_key = traderOrder.UserID + "_" + traderOrder.GroupID;
        int filterAction = 1;

        if ((_strategyConnection.ModelTraderScoreCardJSON == "null")  || (_strategyConnection.ModelTraderScoreCardJSON == "[]")
                    || _strategyConnection.ModelTraderScoreCardJSON.Equals(null) )
                    
        {
            Console.WriteLine($"{YELLOW} NULL ScoreCard Found");
            filterAction = 0;
            Console.ResetColor();
        }                
        else
        {   
            //Console.WriteLine(_strategyConnection.ModelTraderScoreCardJSON);
            filterAction = await EvaluateFilters(await _strategyConnection.GetTraderScoreCard());
        }

        Console.WriteLine(CurrentStrategyAccount.strategy_name  +  " __Filter Action Result: " + filterAction);

        if(!await OkToOpenNewPosition(traderOrder))
        {
            traderOrder.OrderAction = OrderAction.NoAction;
            await LogStrategyData(_trader_key, traderOrder.OrderAction, traderOrder.OrderType); 
            return traderOrder;
        }else
        {
            if (filterAction == 0)
            {
                traderOrder.OrderAction = OrderAction.NoAction;
                await LogStrategyData(_trader_key, traderOrder.OrderAction, traderOrder.OrderType); 
            }    

            // FilterAction > 0 means FOLLOW -- do the same 
            // Nothing Changes
            if (filterAction > 0 )
            {
                await LogStrategyData(_trader_key, traderOrder.OrderAction, traderOrder.OrderType); 
            }

            //  filterAction < 0 means FADE -- do the opposite
            // Reverse Current Order
            if (filterAction < 0)
            {
                if (traderOrder.OrderAction == OrderAction.Buy)
                {
                    traderOrder.OrderAction = OrderAction.Sell;
                    await LogStrategyData(_trader_key, traderOrder.OrderAction, traderOrder.OrderType); 
                } else 
                if (traderOrder.OrderAction == OrderAction.Sell)
                {
                    traderOrder.OrderAction = OrderAction.Buy;
                    await LogStrategyData(_trader_key, traderOrder.OrderAction, traderOrder.OrderType); 
                }
            }

            return traderOrder;
        }
    }


    public async Task<bool> OkToOpenNewPosition(LiveOrderDTO origTraderLiveOrder)
    {   
        bool newPosition = true;
        
        List<LiveOrderDTO> liveOrders = await _strategyConnection.GetStrategyLiveOrders();

        if(liveOrders.Count > (CurrentStrategyAccount.orders_per_direction-1))
        //if(liveOrders.Count > 0)
        {
                newPosition = false;     
                Console.WriteLine($"{YELLOW}{_strategyConnection.GetStrategyProfileKey()}  ***NOT*** OK for new trade");
                Console.ResetColor();
        }else
        {
            Console.WriteLine(_strategyConnection.GetStrategyProfileKey() + "   OK for new trade");
        }

        return  newPosition;
    }

    public async Task LogStrategyData(string trader_k, OrderAction orderAction, OrderType orderType)
    {
        
        string logData = "New Strategy Order: " + _strategyConnection.GetStrategyProfileKey() + "  " + orderAction + "  " + orderType + "  " + trader_k;
        await Task.Run(() =>
        {
            //Console.WriteLine(logData);
        });           
    }

    
}

