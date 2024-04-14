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

        //  122  126 128 129 130

        return IsInFilter130(scoreCard);
    }


    public int IsInFilter130(ScoreCardDTO _scoreCard)
    {

        int min_trades = 10;
        int includeExclude = 0;

        if (_scoreCard.Trades > min_trades && _scoreCard.TotalNetProfit > 0 && ( _scoreCard.PNL_Last5 > _scoreCard.PNL_Last3)   )
        {
            includeExclude = 1;
        }

        if (_scoreCard.Rank == 1)
        {
             includeExclude = 1;
        }

        if (_scoreCard.Rank > 4  && _scoreCard.PNL_Last5 < 0)
        {
             includeExclude = 1;
        }

        if (_scoreCard.Rank < 3  && _scoreCard.PNL_Last5 < _scoreCard.PNL_Last8)
        {
             includeExclude = 1;
        }


        return includeExclude;
    }

    public int IsInFilter129(ScoreCardDTO _scoreCard)
    {

        int min_trades = 10;
        int includeExclude = 0;

        if (_scoreCard.Trades > min_trades && _scoreCard.TotalNetProfit > 0 && ( _scoreCard.PNL_Last5 > _scoreCard.PNL_Last3)   )
        {
            includeExclude = 1;
        }

        if (_scoreCard.Rank == 1)
        {
             includeExclude = 1;
        }

        if (_scoreCard.Rank > 4  && _scoreCard.PNL_Last5 < 0)
        {
             includeExclude = 1;
        }

        return includeExclude;
    }




    public int IsInFilter128(ScoreCardDTO _scoreCard)
    {

        int includeExclude = 0;

        if ( _scoreCard.TotalNetProfit > 0 && ( _scoreCard.PNL_Last5 > _scoreCard.PNL_Last3)   )
        {
            includeExclude = 1;
        }

        if (_scoreCard.Rank == 1)
        {
             includeExclude = 1;
        }

        if (_scoreCard.Rank < 4  && _scoreCard.PNL_Last5 < 0)
        {
             includeExclude = 1;
        }

        return includeExclude;
    }


    public int IsInFilter126(ScoreCardDTO _scoreCard)
    {

        int min_trades = 10;
        int includeExclude = 0;

        if (_scoreCard.Trades > min_trades && _scoreCard.TotalNetProfit > 0 && ( _scoreCard.PNL_Last5 > _scoreCard.PNL_Last3)   )
        {
            includeExclude = 1;
        }

        if (_scoreCard.Rank == 1)
        {
             includeExclude = 1;
        }

        if (_scoreCard.Rank < 4  && _scoreCard.PNL_Last5 < 0)
        {
             includeExclude = 1;
        }

        return includeExclude;
    }


    public int IsInFilter122(ScoreCardDTO _scoreCard)
    {

        int min_trades = 15;
        int includeExclude = 0;

        if (_scoreCard.Trades > min_trades && _scoreCard.TotalNetProfit > 0 && ( _scoreCard.PNL_Last5 > _scoreCard.PNL_Last3)   )
        {
            includeExclude = 1;
        }

        if (_scoreCard.Rank == 1)
        {
             includeExclude = 1;
        }

        if (_scoreCard.Rank < 3  && _scoreCard.PNL_Last5 < 0)
        {
             includeExclude = 1;
        }

        return includeExclude;
    }



}
