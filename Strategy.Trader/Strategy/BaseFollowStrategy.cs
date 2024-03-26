
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using System.Threading.Tasks.Dataflow;

namespace Strategy.Trader.Strategy;

public class BaseFollowStrategy : AbstractStrategyBase , IStrategy
{

    public BaseFollowStrategy( StrategyAccount strategyData) 
    {
        _filters = new List<IStrategyFilter>
        {
            new AllFollowFilter()
        };
        _strategyData = strategyData;
        flowBuffer = new BufferBlock<string>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });
        Task.Run(async () => await ProcessLogBuffer());
    }

    public override Task<int> OnNewData(ModelOrderLogDTO modelOrderLogDTO)
    {
        return OnNewOrder(modelOrderLogDTO);
            
    }

    public async override Task<int> EvaluateFilters(ScoreCardDTO scoreCard)
    {
        int includeExclude = 0;
        ScoreCardDTO _scoreCard = scoreCard;;
        foreach (IStrategyFilter filter in _filters)
        {
            includeExclude = filter.IsInFilter(_scoreCard);
        }
        return includeExclude;
    }



    public async override Task ProcessOrderForStrategy(string strategy_key, NewOrderDTO order)
    {
        if(order.OrderAction != OrderAction.NoAction)
        {
            await Task.Run(() =>
                orderCount++ 
            );
   
        }
    }
}
