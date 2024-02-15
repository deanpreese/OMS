
using OMS.Core.Models;

using OMS.Algorithms.Filters;

namespace OMS.Algorithms.Algos;

    public class OpenCloseAlgo : AbstractOpenClose, IAlgo
    {
        IAlgoFilter algoFilter;

        // ----------------------------------------------------------------------
        public void SetupAlgoTraders()
        {

            algoFilter = new NewAlgoFilter();
            algoFilter.SetupAlgoFilter( UsingDebugging);
            
            /*

            dwl.Information(" ");
            dwl.Information("Setting up Algo Long Trader " + AlgoTraderLong + " under group " + AlgoGroupNumber);
            traderIdLong = authenticator.GetTraderId(AlgoTraderLong,AlgoGroupNumber);
            if ( traderIdLong == 0 )
            {
                traderIdLong = authenticator.AddNewTraderAlgo(AlgoTraderLong,AlgoGroupNumber);
            }

            dwl.Information("Setting up Algo Short Trader " + AlgoTraderShort + " under group " + AlgoGroupNumber);
            traderIdShort = authenticator.GetTraderId(AlgoTraderShort,AlgoGroupNumber);
            if ( traderIdShort == 0 )
            {
                traderIdShort = authenticator.AddNewTraderAlgo(AlgoTraderShort,AlgoGroupNumber);
            }
            */
            
        }

        // ----------------------------------------------------------------------
        public void ProcessExecution(OrderFlow order)
        {
            /*
            int inAlgoFilter = -1;

            inAlgoFilter = algoFilter.IsInAlgoFilter(order.TraderId, order.GroupNumber);

            List<ClosedTrade> trades = hoh.GetXXXOrdersByTrader(order.TraderId,  11, order.GroupNumber );

            int modelingFilter = 0;
            if ( trades.Count() > 9)
            {
                float[] tradeData = { 
                    (float)trades[0].Pnl, 
                    (float)trades[1].Pnl, 
                    (float)trades[2].Pnl, 
                    (float)trades[3].Pnl, 
                    (float)trades[4].Pnl, 
                    (float)trades[5].Pnl,
                    (float)trades[6].Pnl,
                    (float)trades[7].Pnl,
                    (float)trades[8].Pnl
                };
                modelingFilter = mlFilter.PredictNextDirection(tradeData);

                if (( inAlgoFilter == modelingFilter ) && (order.OrderType == 1))
                {
                    inAlgoFilter = modelingFilter;
                    dwl.InfoRed("Algo Filter Match for OPENING Order " + inAlgoFilter + "  |  ML Filter  "  + modelingFilter );   
                }else
                {
                    dwl.Information("No OPEN Action Algo Filter ONE " + inAlgoFilter + "  |  ML Filter  "  + modelingFilter );  
                    inAlgoFilter = 0;
                }

            }

            ProcessOrder(order, inAlgoFilter);
            */
        }

}
