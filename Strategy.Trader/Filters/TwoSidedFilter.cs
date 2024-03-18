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
        return IsInAlgoFilter114();
    }



    public int IsInAlgoFilter114()
    {
        int includeExclude = 0;

        if (_scoreCard.SortinoRatio > 0.5  || _scoreCard.SharpRatio > 0.5)
        {
            includeExclude = 1;
        }

         return includeExclude;
    }



    public int IsInAlgoFilter117()
    {
       int includeExclude = 0;

       if ((_scoreCard.SortinoRatio > 0.5  || _scoreCard.SharpRatio > 0.5 )
            &&  _scoreCard.WinLossRatio > 0.6
            )
        {
            includeExclude = 1;
        }

         return includeExclude;
    }



    public int IsInAlgoFilter116()
    {
        int includeExclude = 0;

        if (_scoreCard.SortinoRatio > 0.6  || _scoreCard.SharpRatio > 0.6 )
           
        {
            includeExclude = 1;
        }

         return includeExclude;
    }



    public int IsInAlgoFilter115()
    {
        int includeExclude = 0;

        if (_scoreCard.SortinoRatio > 0.55  || _scoreCard.SharpRatio > 0.55)
        {
            includeExclude = 1;
        }

         return includeExclude;
    }




    public int IsInAlgoFilter113()
    {
        int includeExclude = 0;

        if (_scoreCard.SortinoRatio > 0.5  || _scoreCard.SharpRatio > 0.5 || _scoreCard.WinLossRatio > 0.6)
        {
            includeExclude = 1;
        }

         return includeExclude;
    }


    public int IsInAlgoFilter112()
    {
        int includeExclude = 0;

        if (_scoreCard.SortinoRatio > 0.35  || _scoreCard.SharpRatio > 0.35 || _scoreCard.WinLossRatio > 0.6)
        {
            includeExclude = 1;
        }

        if (_scoreCard.SortinoRatio < 0  || _scoreCard.SharpRatio < 0 )
        {
            includeExclude = -1;
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
