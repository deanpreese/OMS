using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;

namespace Strategy.Trader.Strategy;

public class BaseCounterStrategy : AbstractStrategy, IStrategy
{
    
    int longCount = 0;
    int shortCount = 0;
    int netPositions = 0;

    List<int> longList = new List<int>();
    List<int> shortList = new List<int>();

    List<int> netList = new List<int>();

    List<double> longAverageList = new List<double>();
    List<double> shortAverageList = new List<double>();



    public BaseCounterStrategy(IStrategyConnection strategyConnection) : base(strategyConnection)
    {
        _filters = new List<IStrategyFilter>
        {
            new AllFollowFilter()
        };
    }

    public override async  Task<NewOrderDTO> OnNewData(LiveOrderDTO traderLiveOrderDTO)
    {

        NewOrderDTO _mapped_new_order = await SharedMapping.MapLiveOrderDTOLiveToNewDTO(traderLiveOrderDTO);    
        _mapped_new_order.OrderAction = OrderAction.NoAction;
        
        //Console.WriteLine(CurrentStrategyAccount.logid + "  " + orderCount);

        
        int netPositions  = await ProcessCounter(traderLiveOrderDTO);
        if(netPositions > 9 || netPositions < -9) 
        {
            NewOrderDTO newOrder = await ProcessNewData(traderLiveOrderDTO);
            _mapped_new_order  = newOrder;
        }
        
        return _mapped_new_order;            
    }

    public async override Task<int> EvaluateFilters(ScoreCardDTO scoreCard)
    {
        return await Task.FromResult(1);
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
        
        //await AddToLogBuffer("NET: " + netPositions + "  AvePos10  " + netAve + "      Long: " + longCount + "  Short: " + shortCount + "       LongAve: " + longAve + "     ShortAve: " + shortAve);

        return await Task.FromResult(netPositions);

    }

}
