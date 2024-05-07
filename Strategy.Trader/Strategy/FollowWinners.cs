
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Strategy.Trader.Strategy;

public class FollowWinners : AbstractStrategy , IStrategy
{

    public FollowWinners(IStrategyConnection strategyConnection) : base(strategyConnection)
    {
        _filters = new List<IStrategyFilter>
        {
            new AllFollowFilter()
        };
    }

    public override async  Task<NewOrderDTO> OnNewData(LiveOrderDTO traderLiveOrderDTO)
    {
        /*
        NewOrderDTO _mapped_new_order = await SharedMapping.MapLiveOrderDTOLiveToNewDTO(traderLiveOrderDTO);    
        _mapped_new_order.OrderAction = OrderAction.NoAction;
        Console.WriteLine(ThisStrategyAccount.logid + "  " + orderCount);
        return _mapped_new_order;   
        */

        return await ProcessNewData(traderLiveOrderDTO);

    }

    public async override Task<int> EvaluateFilters(ScoreCardDTO scoreCard)
    {
        int includeExclude = 0;
        foreach (IStrategyFilter filter in _filters)
        {
            includeExclude = filter.IsInFilter(scoreCard);
        }
        return await Task.FromResult(includeExclude);
    }  

}
