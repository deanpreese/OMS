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
        return IsInAlgoFilter113();
    }
    public int IsInAlgoFilter113()
    {
        int includeExclude = 0;

        
        double Set3 = _scoreCard.PNL_Last3;
        double Set5 = _scoreCard.PNL_Last5;
        double Set13 = _scoreCard.PNL_Last13;

        if (_scoreCard.WinLossRatio > .65)
        {
            includeExclude = 1;
        }
        

        return includeExclude;
    }




    public int IsInAlgoFilter112()
    {
        int includeExclude = 0;


        if (_scoreCard.Winners > _scoreCard.Losers)
        {
            if (_scoreCard.GrossProfit > Math.Abs((double)_scoreCard.GrossLoss))
            {
                includeExclude = 1;
            }

        }
        else
        {
            double Set3 = _scoreCard.PNL_Last3;
            double Set5 = _scoreCard.PNL_Last5;
            double Set13 = _scoreCard.PNL_Last13;

            if (_scoreCard.WinLossRatio < .45)
            {
                // 5 SMA < 13 SMA -->>  Losing
                if (Set5 < Set13 && Set3 < Set5)
                {
                    if (Set5 < 0)
                    {
                        includeExclude = -1;
                    }
                }

            }

            if (_scoreCard.WinLossRatio > .5)
            {
                // 5 SMA > 13 SMA -->>  Winning
                if (Set5 > Set13 && Set3 > Set5)
                {
                    if (Set5 > 0)
                    {
                        includeExclude = 1;
                    }
                }
            }
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
