using System;
using System.Linq;
using System.Collections.Generic;


namespace OMS.AlgoManager.Filters;

public class AlgoFilterSeptThreeZero : IAlgoFilter
{
    bool useDebugging;
    int statsLength;
    int setOne ;
    int setTwo;
    int setThree;

    public void SetupAlgoFilter( bool debugging)
    {
        useDebugging = debugging;

        statsLength = -1;  // -1
        setOne = 3;  // 3
        setTwo = 5;  // 5
        setThree = 10;  // 10

    }


    public int IsInAlgoFilter(int traderId, int groupNumber)
    {
        return 0;
    }

    /*
  
    public int IsInAlgoFilter99(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        TraderStatistics ts = sb.GenerateStatistics(traderId, statsLength, groupNumber);

        if ( ts.Trades < 13 ) 
        { 
            includeExclude = 0;
            return includeExclude;
        }

        //Winning Trader overall
        if(ts.GrossProfit > Math.Abs((double)ts.GrossLoss))
        {
            includeExclude = 1;
        }else
        {
            double Set3 =  sb.GetTradesPNL( traderId, setOne);
            double Set5 =  sb.GetTradesPNL( traderId, setTwo);
            double Set13 = sb.GetTradesPNL( traderId, setThree);

            // 5 SMA < 13 SMA -->>  Losing
            if( Set5 < Set13 && Set3 < Set5)
            {
                if(Set5 < 0)        
                {
                   includeExclude = -1; 
                }
            }
                
            // 5 SMA > 13 SMA -->>  Winning
            if( Set5 > Set13 && Set3 > Set5) 
            {
                if(Set5 > 0)        
                {
                   includeExclude = 1; 
                }
            }
            
        }

        
        return includeExclude;
    }
 

    public int IsInAlgoFilter98(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        TraderStatistics ts = sb.GenerateStatistics(traderId, statsLength, groupNumber);

        if ( ts.Trades < 13 ) 
        { 
            includeExclude = 0;
            return includeExclude;
        }


        if ( ts.Winners > ts.Losers)
        {
            if(ts.GrossProfit > Math.Abs((double)ts.GrossLoss))
            {
                includeExclude = 1;
            }

        }else
        {
            double Set3 =  sb.GetTradesPNL( traderId, setOne);
            double Set5 =  sb.GetTradesPNL( traderId, setTwo);
            double Set13 = sb.GetTradesPNL( traderId, setThree);

            if(ts.WinLossRatio < .45)
            {
                // 5 SMA < 13 SMA -->>  Losing
                if( Set5 < Set13 && Set3 < Set5)
                {
                    if(Set5 < 0)        
                    {
                    includeExclude = -1; 
                    }
                }
                    
                // 5 SMA > 13 SMA -->>  Winning
                if( Set5 > Set13 && Set3 > Set5) 
                {
                    if(Set5 > 0)        
                    {
                    includeExclude = 1; 
                    }
                }
            }
            
        }

        
        return includeExclude;
    }


    public int IsInAlgoFilter97(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        TraderStatistics ts = sb.GenerateStatistics(traderId, statsLength, groupNumber);

        if ( ts.Trades < 13 ) 
        { 
            includeExclude = 0;
            return includeExclude;
        }


        if ( ts.Winners > ts.Losers)
        {
            if(ts.GrossProfit > Math.Abs((double)ts.GrossLoss))
            {
                includeExclude = 1;
            }

        }else
        {
            double Set3 =  sb.GetTradesPNL( traderId, setOne);
            double Set5 =  sb.GetTradesPNL( traderId, setTwo);
            double Set13 = sb.GetTradesPNL( traderId, setThree);

            if(ts.WinLossRatio < .45)
            {
                // 5 SMA < 13 SMA -->>  Losing
                if( Set5 < Set13 && Set3 < Set5)
                {
                    if(Set5 < 0)        
                    {
                    includeExclude = -1; 
                    }
                }

            }

            if(ts.WinLossRatio > .5 )                        
            {
                // 5 SMA > 13 SMA -->>  Winning
                if( Set5 > Set13 && Set3 > Set5) 
                {
                    if(Set5 > 0)        
                    {
                    includeExclude = 1; 
                    }
                }
            }
        }
        
        return includeExclude;
    }

    public int IsInAlgoFilter96(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        TraderStatistics ts = sb.GenerateStatistics(traderId, statsLength, groupNumber);

        if ( ts.Trades < 13 ) 
        { 
            includeExclude = 0;
            return includeExclude;
        }


        if ( ts.Winners > ts.Losers)
        {
            if(ts.GrossProfit > Math.Abs((double)ts.GrossLoss))
            {
                includeExclude = 1;
            }

        }else
        {
            double Set3 =  sb.GetTradesPNL( traderId, setOne);
            double Set5 =  sb.GetTradesPNL( traderId, setTwo);
            double Set13 = sb.GetTradesPNL( traderId, setThree);

            if(ts.WinLossRatio < .40)
            {
                // 5 SMA < 13 SMA -->>  Losing
                if( Set5 < Set13 && Set3 < Set5)
                {
                    if(Set5 < 0)        
                    {
                    includeExclude = -1; 
                    }
                }

            }

            if(ts.WinLossRatio > .55 )                        
            {
                // 5 SMA > 13 SMA -->>  Winning
                if( Set5 > Set13 && Set3 > Set5) 
                {
                    if(Set5 > 0)        
                    {
                    includeExclude = 1; 
                    }
                }
            }
        }
        
        return includeExclude;
    }

    public int IsInAlgoFilter95(int traderId, int groupNumber)
    {
        int f1 = IsInAlgoFilter99(traderId, groupNumber);
        int f2 = IsInAlgoFilter98(traderId, groupNumber);
        int f3 = IsInAlgoFilter97(traderId, groupNumber);
        int f4 = IsInAlgoFilter96(traderId, groupNumber);

        int total = f1 + f2 + f3 + f4;

        if( total > 2)
        {
            return 1;
        }

        if( total < -2)
        {
            return -1;
        }

        return 0;
    }

    public int IsInAlgoFilter94(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        List<ClosedTrade> TradesOne = hoh.GetXXXOrdersByTrader(traderId,  10, groupNumber );
        if ( TradesOne.Count < 3 ) 
        { 
            includeExclude = 0;
            return includeExclude;
        }
        
        // ------------------
        double[] aveVsEquity = sb.GetEquityAveComparision(traderId, groupNumber, 5);
        double Set3 =  sb.GetTradesPNL( traderId, setOne);
        double Set5 =  sb.GetTradesPNL( traderId, setTwo);
        double Set10 = sb.GetTradesPNL( traderId, setThree);

        if(aveVsEquity[0] > 0)
        {
            if ( aveVsEquity[0] > aveVsEquity[1] )
            {
               
                if (Set3  > 0)  { includeExclude = 1; }  
                if (Set5  > 0)  { includeExclude = 1; }
                if (Set10  > 0) { includeExclude =  0; }

                 return includeExclude;
            }else
            {
                return -1;
            }
        }

        if(aveVsEquity[0] < 0)
        {
            if ( aveVsEquity[0] < aveVsEquity[1] )
            {
                if (Set3  < 0)  { includeExclude = -1; }  
                if (Set5  < 0)  { includeExclude =  1; }
                if (Set10  < 0) { includeExclude =  0 ;  }

                 return includeExclude;
            }else
            {
                return 0;
            }
        }
        
        return includeExclude;
    }



    public int IsInAlgoFilter93(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        TraderStatistics ts = sb.GenerateStatistics(traderId, statsLength, groupNumber);

        if ( ts.Trades < 13 ) 
        { 
            includeExclude = 0;
            return includeExclude;
        }

        
        // Mostly losing trades
        if ( ts.WinLossRatio < .4 )
        {
            if (ts.CurrentLosingStreak > 0)
            {
                if ( ts.CurrentLosingStreak < ts.LargestLosingStreak)        
                {
                    includeExclude = -1;
                }
            }
        }
        
        
        return includeExclude;
    }


    public int IsInAlgoFilter92(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        TraderStatistics ts = sb.GenerateStatistics(traderId, statsLength, groupNumber);

        if ( ts.Trades < 13 ) 
        { 
            includeExclude = 0;
            return includeExclude;
        }

        
        // Mostly losing trades
        if ( ts.WinLossRatio < .25 )
        {
            if (ts.CurrentLosingStreak > 0)
            {
                if ( ts.CurrentLosingStreak < ts.LargestLosingStreak)        
                {
                    includeExclude = -1;
                }
            }
        }
        
        
        return includeExclude;
    }

    public int IsInAlgoFilter91(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        TraderStatistics ts = sb.GenerateStatistics(traderId, statsLength, groupNumber);

        if ( ts.Trades < 13 ) 
        { 
            includeExclude = 0;
            return includeExclude;
        }

        
        // Mostly losing trades
        if ( ts.WinLossRatio < .25 )
        {
            includeExclude = -1;
        }
        
        return includeExclude;
    }


    public int IsInAlgoFilter90(int traderId, int groupNumber)
    {

        TraderStatistics ts = sb.GenerateStatistics(traderId, statsLength, groupNumber);

        int f1 = IsInAlgoFilter99(traderId, groupNumber);
        int f2 = IsInAlgoFilter98(traderId, groupNumber);
        int f3 = IsInAlgoFilter97(traderId, groupNumber);
        int f4 = IsInAlgoFilter96(traderId, groupNumber);

        int total = f1 + f2 + f3 + f4;

        if( total > 2  && ts.WinLossRatio < .55 )
        {
            return 1;
        }

        if( total < -2 && ts.WinLossRatio < .35 )
        {
            return -1;
        }

        return 0;
    }


    public int IsInAlgoFilter89(int traderId, int groupNumber)
    {

        TraderStatistics ts = sb.GenerateStatistics(traderId, statsLength, groupNumber);

        int f1 = IsInAlgoFilter99(traderId, groupNumber);
        int f2 = IsInAlgoFilter98(traderId, groupNumber);
        int f3 = IsInAlgoFilter97(traderId, groupNumber);
        int f4 = IsInAlgoFilter96(traderId, groupNumber);

        int total = f1 + f2 + f3 + f4;

        if( total > 2  && ts.WinLossRatio < .70 )
        {
            return 1;
        }

        if( total < -2 && ts.WinLossRatio < .40 )
        {
            return -1;
        }

        return 0;
    }


     public int IsInAlgoFilter88(int traderId, int groupNumber)
    {
        
        int includeExclude = 0;
        
        TraderStatistics ts = sb.GenerateStatistics(traderId, statsLength, groupNumber);

        if ( ts.Trades < 13 ) 
        { 
            return 0;
        }


        if ( ts.Winners > ts.Losers)
        {
            if(ts.GrossProfit > Math.Abs((double)ts.GrossLoss))
            {
                //includeExclude = 1;
            }

        }else
        {
            //double Set3 =  sb.GetTradesPNL( traderId, setOne)/setOne;
            //double Set5 =  sb.GetTradesPNL( traderId, setTwo)/setTwo;
            //double Set13 = sb.GetTradesPNL( traderId, setThree)/setThree;

            double Set3 =  sb.GetTradesPNL( traderId, setOne);
            double Set5 =  sb.GetTradesPNL( traderId, setTwo);
            double Set13 = sb.GetTradesPNL( traderId, setThree);


            if(ts.WinLossRatio < .45)
            {
                // 5 SMA < 13 SMA -->>  Losing
                if( Set5 < Set13 && Set3 < Set5)
                {
                    if(Set5 < 0)        
                    {
                        includeExclude = -1; 
                    }
                }
                    
                // 5 SMA > 13 SMA -->>  Winning
                if( Set5 > Set13 && Set3 > Set5) 
                {
                    if(Set3 > 0)        
                    {
                        //includeExclude = 1; 
                    }
                }
            }
            
        }

        if ( ts.WinLossRatio < .25 )
        {
            includeExclude = -1;
        }

        
        return includeExclude;
    }
    */
}