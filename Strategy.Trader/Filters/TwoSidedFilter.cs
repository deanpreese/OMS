using Strategy.Trader.Abstractions;
using OMS.Core.Models;
//
//   Group 89
//



namespace Strategy.Trader.Filters;

public class TwoSidedFilter : AbstractStrategyFilter, IStrategyFilter
{

    public TwoSidedFilter(UserProfile userProfile, ScoreCard scoreCard) : base(userProfile, scoreCard)
    {
    }

    public int IsInAlgoFilter()
    {
        return IsInAlgoFilter100();
    }


    public int IsInAlgoFilter100()
    {
        int includeExclude = 0;

        if (_scoreCard.Trades < 15)
        {
            return 1;
        }

        if (_scoreCard.TotalNetProfit > 0)
        {
            return 1;
        }

        double Set3 = _scoreCard.PNL_Last3;
        double Set5 = _scoreCard.PNL_Last5;
        double Set13 = _scoreCard.PNL_Last13;

        if (Set5 < Set13 && Set3 < Set5)
        {
            if (Set5 < 0)
            {
                includeExclude = -1;
            }
        }

        if (Set5 > Set13 && Set3 > Set5)
        {
            if (Set5 > 0)
            {
                includeExclude = 1;
            }
        }



        return includeExclude;
    }



    public int IsInAlgoFilter99()
    {
        int includeExclude = 0;

        //Winning Trader overall
        if (_scoreCard.GrossProfit > Math.Abs((double)_scoreCard.GrossLoss))
        {
            includeExclude = 1;
        }
        else
        {
            double Set3 = _scoreCard.PNL_Last3;
            double Set5 = _scoreCard.PNL_Last5;
            double Set13 = _scoreCard.PNL_Last13;

            if (Set5 < Set13 && Set3 < Set5)
            {
                if (Set5 < 0)
                {
                    includeExclude = -1;
                }
            }

            if (Set5 > Set13 && Set3 > Set5)
            {
                if (Set5 > 0)
                {
                    includeExclude = 1;
                }
            }

        }

        return includeExclude;
    }


    public int IsInAlgoFilter98()
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


    public int IsInAlgoFilter97()
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



}
