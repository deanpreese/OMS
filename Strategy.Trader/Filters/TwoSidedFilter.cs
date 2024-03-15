using Strategy.Trader.Abstractions;
using OMS.Core.Models;
//
//   Group 89
//



namespace Strategy.Trader.Filters;

public class TwoSidedFilter : IStrategyFilter
{
    ScoreCard _scoreCard;


    public int IsInFilter(ScoreCard scoreCard)
    {
        _scoreCard = scoreCard;
        return IsInAlgoFilter111();
    }


    public int IsInAlgoFilter112()
    {
        int includeExclude = 0;

        if (_scoreCard.SortinoRatio > 0.5  || _scoreCard.SharpRatio > 0.5 )
        {
            includeExclude = 1;
        }
    

        return includeExclude;
    }


    public int IsInAlgoFilter111()
    {
        int includeExclude = 0;

        if (_scoreCard.SortinoRatio > 0.35  || _scoreCard.SharpRatio > 0.35 || _scoreCard.WinLossRatio > 0.6)
        {
            includeExclude = 1;
        }
    

        return includeExclude;
    }


}
