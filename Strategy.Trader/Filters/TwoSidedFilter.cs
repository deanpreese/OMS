using Strategy.Trader.Abstractions;

using OMS.SharedKernel.DTO;
using OMS.SharedKernel.Common;


namespace Strategy.Trader.Filters;

public class TwoSidedFilter : ScreenColorBase, IStrategyFilter
{
    ScoreCardDTO _scoreCard;


    public int IsInFilter(ScoreCardDTO scoreCard)
    {
        _scoreCard = scoreCard;
        return IsInFilter115(scoreCard);
    }



    public int IsInFilter115(ScoreCardDTO _scoreCard)
    {
        int includeExclude = 0;
        if ((_scoreCard.SortinoRatio > 0.5  || _scoreCard.SharpRatio > 0.5) && _scoreCard.Rank < 3)
        {
            includeExclude = 1;
        }

         return includeExclude;
    }


    public int IsInFilter114(ScoreCardDTO _scoreCard)
    {
        int includeExclude = 0;
       
        Console.WriteLine($" {MAGENTA} ---- {_scoreCard.SortinoRatio} { _scoreCard.SharpRatio} ----");
        Console.ResetColor();

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
