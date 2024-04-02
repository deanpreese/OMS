
using Strategy.Trader.Abstractions;
using Strategy.Trader.Filters;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;

namespace Strategy.Trader.Strategy;

public class OpenCloseStrategy : AbstractStrategy , IStrategy
{

    public OpenCloseStrategy(IStrategyConnection strategyConnection) : base(strategyConnection)
    {
        _filters = new List<IStrategyFilter>
        {
            new TwoSidedFilter()
        };
    }


    public override async  Task<NewOrderDTO> OnNewData(LiveOrderDTO traderLiveOrderDTO)
    {
        return await ProcessNewData(traderLiveOrderDTO);
    }


    public override async Task<int> EvaluateFilters(ScoreCardDTO scoreCard)
    {
      
        int includeExclude = 0;

        foreach (IStrategyFilter filter in _filters)
        {
            includeExclude = filter.IsInFilter(scoreCard);
        }

        //Console.WriteLine($" {YELLOW} ---- {scoreCard.Trades} { scoreCard.Winners} ----");
        //Console.WriteLine($" {YELLOW} ---- {scoreCard.SortinoRatio} { scoreCard.SharpRatio} ----");
        //Console.ResetColor();

        /*
        if (includeExclude == 0)
        {
            Console.WriteLine(_strategyConnection.ModelTraderScoreCardJSON);
        }
        */

        return await Task.FromResult(includeExclude);
    }

}
