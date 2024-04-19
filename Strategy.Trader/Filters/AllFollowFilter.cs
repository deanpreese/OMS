
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.Trader.Abstractions;

namespace Strategy.Trader.Filters;

public class AllFollowFilter : ScreenColorBase, IStrategyFilter
{
    ScoreCardDTO _scoreCard;


    public int IsInFilter(ScoreCardDTO scoreCard)
    {
        return IsInFilter9(scoreCard);
    }
    
    public int IsInFilter9(ScoreCardDTO scoreCard)
    {
        //58	32	26	0.151761	0.318956
        int includeExclude = 0;
        int min_trades = 30;

        _scoreCard = scoreCard;
        if (_scoreCard.TotalNetProfit > 0 )
        {
            double confidenceLevel = 0.95; 
            double successProbability = _scoreCard.WinLossRatio;

            min_trades = (int)(Math.Ceiling(Math.Log(1 - confidenceLevel) / Math.Log(1 - successProbability))*10);

            Console.WriteLine("Min Trades: " + min_trades);

            if (_scoreCard.TotalNetProfit > 0 && _scoreCard.SortinoRatio > 0.0 && _scoreCard.SharpRatio > 0.0)
                includeExclude =  1;

            if (_scoreCard.Trades < min_trades)
                includeExclude =  0;

        }

        return includeExclude;

    } 



    public int IsInFilter8(ScoreCardDTO scoreCard)
    {

        //70	36	34	0.083679	0.160695
        int includeExclude = 0;
        int min_trades = 30;

        _scoreCard = scoreCard;
        if (_scoreCard.WinLossRatio > .51 )
        {
            double confidenceLevel = 0.90; 
            double successProbability = _scoreCard.WinLossRatio;

            min_trades = (int)(Math.Ceiling(Math.Log(1 - confidenceLevel) / Math.Log(1 - successProbability))*10);

            Console.WriteLine("Min Trades: " + min_trades);

            if (_scoreCard.TotalNetProfit > 0 && _scoreCard.SortinoRatio > 0.0 && _scoreCard.SharpRatio > 0.0)
                includeExclude =  1;

            if (_scoreCard.Trades < min_trades)
                includeExclude =  0;

        }

        return includeExclude;

    } 


    public int IsInFilter7(ScoreCardDTO scoreCard)
    {

        //68	35	33	0.111463	0.209612

        int includeExclude = 0;
        int min_trades = 30;

        _scoreCard = scoreCard;
        if (_scoreCard.WinLossRatio > .50 )
        {
            double confidenceLevel = 0.90; 
            double successProbability = _scoreCard.WinLossRatio;

            min_trades = (int)(Math.Ceiling(Math.Log(1 - confidenceLevel) / Math.Log(1 - successProbability))*10);

            Console.WriteLine("Min Trades: " + min_trades);

            if (_scoreCard.TotalNetProfit > 0 && _scoreCard.SortinoRatio > 0.0 && _scoreCard.SharpRatio > 0.0)
                includeExclude =  1;

            if (_scoreCard.Trades < min_trades)
                includeExclude =  0;

        }

        return includeExclude;

    } 



    public int IsInFilter6(ScoreCardDTO scoreCard)
    {

        // 58	32	26	0.151761	0.318956

        int includeExclude = 0;
        int min_trades = 30;

        _scoreCard = scoreCard;
        if (_scoreCard.WinLossRatio > .5 )
        {
            double confidenceLevel = 0.95; 
            double successProbability = _scoreCard.WinLossRatio;

            min_trades = (int)(Math.Ceiling(Math.Log(1 - confidenceLevel) / Math.Log(1 - successProbability))*10);

            Console.WriteLine("Min Trades: " + min_trades);

            if (_scoreCard.TotalNetProfit > 0 && _scoreCard.SortinoRatio > 0.0 && _scoreCard.SharpRatio > 0.0)
                includeExclude =  1;

            if (_scoreCard.Trades < min_trades)
                includeExclude =  0;

        }

        return includeExclude;

    } 



    public int IsInFilter5(ScoreCardDTO scoreCard)
    {
        //73	36	37	0.086429	0.183739
        int includeExclude = 0;
        _scoreCard = scoreCard;

        if (_scoreCard.TotalNetProfit > 0 && _scoreCard.SortinoRatio > 0.0 && _scoreCard.SharpRatio > 0.0)
            includeExclude = 1;   

        if (_scoreCard.Trades < 20)
             includeExclude =  0;

        return includeExclude;

    } 



    public int IsInFilter4(ScoreCardDTO scoreCard)
    {

        // 63	33	30	0.120427	0.251545
        int includeExclude = 0;

        _scoreCard = scoreCard;

        if (_scoreCard.TotalNetProfit > 0 && _scoreCard.SortinoRatio > 0.0 && _scoreCard.SharpRatio > 0.0)
            includeExclude = 1;   

        if (_scoreCard.Trades < 30)
             includeExclude =  0;

        return includeExclude;

    } 


    public int IsInFilter3(ScoreCardDTO scoreCard)
    {
        //96	47	49	0.042894	0.078756
        _scoreCard = scoreCard;

        if (_scoreCard.TotalNetProfit > 0 && _scoreCard.SortinoRatio > 0.0 && _scoreCard.SharpRatio > 0.0)
        {
            return 1;
        }


        return 0;

    } 

 public int IsInFilter2(ScoreCardDTO scoreCard)
    {
        //17	7	10	-0.144615	-0.19279
        _scoreCard = scoreCard;

        if (_scoreCard.TotalNetProfit > 0 && _scoreCard.SortinoRatio > 0.5 && _scoreCard.SharpRatio > 0.5)
        {
            return 1;
        }


        return 0;

    }    

    public int IsInFilter1(ScoreCardDTO scoreCard)
    {
        //98	49	49	0.052266	0.094902

        _scoreCard = scoreCard;

        if (_scoreCard.TotalNetProfit > 0 )
        {
            return 1;
        }


        return 0;

    }
}