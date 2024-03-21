using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;
using Strategy.Trader.Models;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.SharedKernel.Grains;
using System.Threading.Tasks.Dataflow;

namespace Strategy.Trader.Strategy;

public class BaseCounterStrategy : AbstractStrategyBase , IStrategy
{
    IClusterClient newClusterClient;
   
    int longCount = 0;
    int shortCount = 0;
    int netPositions = 0;

    List<int> longList = new List<int>();
    List<int> shortList = new List<int>();

    List<int> netList = new List<int>();

    List<double> longAverageList = new List<double>();
    List<double> shortAverageList = new List<double>();



    public BaseCounterStrategy(IClusterClient clusterClient, StrategyAccount strategyData) 
    {
        newClusterClient = clusterClient;
        _filters = new List<IStrategyFilter>
        {
            new AllFollowFilter()
        };
        _strategyData = strategyData;
        flowBuffer = new BufferBlock<string>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });
        Task.Run(async () => await ProcessLogBuffer());
    }

    public override async Task<NewOrderDTO> OnNewOrder(LiveOrderDTO _orig_live_order)
    {

        string _trader_key = _orig_live_order.UserID + "_" + _orig_live_order.GroupID;
        _strategy_key = _strategyData.strategy_traderId + "_" + _strategyData.group;

        NewOrderDTO _mapped_new_order = await SharedMapping.MapLiveOrderDTOLiveToNewDTO(_orig_live_order);    
        _mapped_new_order.OrderAction = OrderAction.NoAction;
        
        int netPositions  = await ProcessCounter(_orig_live_order);

        if(netPositions > 9 || netPositions < -9) 
        {
            ITraderInfoGrain traderInfoGrain = newClusterClient.GetGrain<ITraderInfoGrain>(_trader_key);
            IStrategyGrain strategyGrain = newClusterClient.GetGrain<IStrategyGrain>(_strategy_key);

            NewOrderDTO newOrder = await OnNewOrder(_orig_live_order, traderInfoGrain, strategyGrain);
            _mapped_new_order  = newOrder;
        }

        return _mapped_new_order;            
    }

    public async override Task<int> EvaluateFilters(string trader_key, string strategy_key, 
        ITraderInfoGrain traderGrain, IStrategyGrain strategyGrain)
    {
        return await Task.FromResult(1);
    }



    public async override Task ProcessOrderForStrategy(string strategy_key, NewOrderDTO order)
    {
       if(order.OrderAction != OrderAction.NoAction)
        {
            IOrderGrain orderGrain = newClusterClient.GetGrain<IOrderGrain>("T"+_strategyData.strategy_traderId + "-"+ orderCount);
            
            await orderGrain.ProcessOrder(order);
            
            orderCount++;
        }
    }


    public async  Task<int> ProcessCounter(LiveOrderDTO order)
    {

        switch (order.OrderAction)
        {
            case OrderAction.Buy:
                longCount++;
                shortCount = 0;
                netPositions++;
                break;
            case OrderAction.Sell:
                shortCount++;
                longCount = 0;
                netPositions--;
                break;
        }


        netList.Add(netPositions);

        longList.Add(longCount);
        shortList.Add(shortCount);    

        shortList.Reverse();
        longList.Reverse();


        int aveCount = 10;
        
        double shortAve = shortList.Take(aveCount).Average();
        double longAve = longList.Take(aveCount).Average();

        longAverageList.Add(longAve);
        shortAverageList.Add(shortAve);

        netList.Reverse();
        double netAve = netList.Take(aveCount).Average();
        
        await AddToLogBuffer("NET: " + netPositions + "  AvePos10  " + netAve + "      Long: " + longCount + "  Short: " + shortCount + "       LongAve: " + longAve + "     ShortAve: " + shortAve);

        return await Task.FromResult(netPositions);

    }

}
