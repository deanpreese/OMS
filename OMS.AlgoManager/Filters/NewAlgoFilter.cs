using OMS.Core.Models;

namespace OMS.AlgoManager.Filters;


public class NewAlgoFilter : IAlgoFilter
{
    bool useDebugging;

    public void SetupAlgoFilter( bool debugging)
    {
        
        useDebugging = debugging;
        //sb = new StatisticsBuilder(dwl);
        //hoh = new HistOrderHelper();
        
    }

    
    public int IsInAlgoFilter(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        /*
        
        List<ClosedTrade> TradesOne = hoh.GetXXXOrdersByTrader(traderId,  10, groupNumber );
        if ( TradesOne.Count < 3 ) 
        { 
            includeExclude = 0;
            return includeExclude;
        }
        
        // ------------------
        double[] aveVsEquity = sb.GetEquityAveComparision(traderId, groupNumber, 5);
        double Set3 =  sb.GetTradesPNL( traderId, 3);
        double Set5 =  sb.GetTradesPNL( traderId, 5);
        double Set10 = sb.GetTradesPNL( traderId, 10);

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
        */
        return includeExclude;
    }

              

}
