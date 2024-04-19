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


public abstract class AbstractStrategyOG :  ScreenColorBase
{

    // Strategy and Trader Properties
    public IStrategyConnection _strategyConnection;
    public StrategyAccount CurrentStrategyAccount { get; set; }
    public List<IStrategyFilter> _filters = new List<IStrategyFilter>();

    public abstract Task<int> EvaluateFilters(ScoreCardDTO scoreCard);
    public abstract Task<NewOrderDTO> OnNewData(LiveOrderDTO traderLiveOrderDTO);
    public int orderCount;

    public AbstractStrategyOG(IStrategyConnection strategyConnection)
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
            
            /*
            Console.WriteLine(CurrentStrategyAccount.logid + "  " + orderCount);
            if ( _strategyConnection.ModelTraderLiveOrderDTO.OrderType == OrderType.OPEN)
            {
                Console.WriteLine(orderCount + " " + CurrentStrategyAccount.logid + " OPEN " + _strategyConnection.ModelTraderLiveOrderJSON);
                Console.WriteLine(orderCount + " " + CurrentStrategyAccount.logid + " OPEN " );
            }

            if ( _strategyConnection.ModelTraderLiveOrderDTO.OrderType == OrderType.CLOSE)
            {
                Console.WriteLine(orderCount + " " + CurrentStrategyAccount.logid + " CLOSE " + _strategyConnection.ModelTraderClosedTradeJSON);
                Console.WriteLine(orderCount + " " + CurrentStrategyAccount.logid + " CLOSE " );
            }
            Console.WriteLine("-----------------");
            */
            
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
        
        string _trader_key = traderOrder.UserID + "_" + traderOrder.GroupID;
        
        //Console.WriteLine(CurrentStrategyAccount.strategy_name + " New Order for Trader " + _trader_key + "  " + traderOrder.OrderAction + "  " + traderOrder.OrderType) ;

        if (traderOrder.OrderType == OrderType.OPEN)
        {
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

        if (traderOrder.OrderType == OrderType.CLOSE)
        {
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
                    }
                    if (liveStrategyOrder.OrderAction == OrderAction.Sell)
                    {
                        traderOrder.OrderAction = OrderAction.Buy;
                        await LogStrategyData(_trader_key, traderOrder.OrderAction, traderOrder.OrderType); 
                    }
                }
            }else
            {
                await LogStrategyData(_trader_key, traderOrder.OrderAction, OrderType.NONE); 
                Console.WriteLine(CurrentStrategyAccount.strategy_name +  " NULL ORDER: " + liveOrders.Count);
            }            
        }

        return traderOrder;
    }



    public async Task<bool> OkToOpenNewPosition(LiveOrderDTO origTraderLiveOrder)
    {   
        bool newPosition = true;
        
        List<LiveOrderDTO> liveOrders = await _strategyConnection.GetStrategyLiveOrders();
        
        //Console.WriteLine( _strategyConnection.GetStrategyProfileKey() + "  " + _strategyConnection.GetStrategyAccount().strategy_name +  " Strategy Orders Count for New: " + liveOrders.Count);

        if(liveOrders.Count > 0)
        {
            //int ordersSameDirection = liveOrders.FindAll(x => x.OrderAction == order.OrderAction).Count();
            //if (_strategyData.orders_per_direction >= ordersSameDirection)
            //{
                newPosition = false;     

                Console.WriteLine(_strategyConnection.GetStrategyProfileKey() + "   ***NOT***  OK for new trade");
            //}
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
