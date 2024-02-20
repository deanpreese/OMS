using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Algo.Algorithms.Filters;
using Algo.Algorithms.Models;

namespace Algo.Algorithms.Utility
{
    public class LeverageCalculator
    {

        private ColoredScreenWriter csw = new ColoredScreenWriter(0);

        // ----------------------------------------------------------
        public OrderData CalculateLeverageQuantity(OrderData LeverageOrder, string orignalTraderID, int orignalOrderDirection, string algoTrader, int baseLeverage, bool useOverride)
        {

            OrderData leverageOrder = LeverageOrder;

            string trader = orignalTraderID;
            leverageOrder.TraderID = algoTrader;
            int oa = orignalOrderDirection;

            bool reversed = false;

            int tradeWindow = 14;
            int threshold = 14;

            TradesWindowFilter twf = new TradesWindowFilter();
            int validTrades = twf.ValidTradesinWindow(14, trader);

            // the trader has enough trades within a given period of time
            if (validTrades > threshold)
            {

                //---------------
                // APPLY Filters
                //---------------

                // Get the GROUP leverage modifier
                GroupLeverageFilter glf = new GroupLeverageFilter();
                int grpLeverage = glf.CalculateGroupLeverage(trader);

                //Get the Moving Average modifier
                MovAveFilter mov = new MovAveFilter();
                int movLeverage = mov.CalculateLeverageModifier(trader, 7);
                double lastPNL = mov.LastPNL;
                double lastAve = mov.LastAverage;

                int quantity = (baseLeverage * grpLeverage) + (baseLeverage * movLeverage);

                if (quantity <= 0)
                {
                    quantity = baseLeverage;
                }

                leverageOrder.Quantity = quantity;

                //---------------
                // APPLY Contra
                //---------------

                // This trader is in the BEST X % 
                // Order will not be reversed
                if (glf.GroupContraRating > 0)
                {

                    // reverse with base leverage

                    leverageOrder.Quantity = baseLeverage;
                    leverageOrder.OrderAction = EngineUtilities.ConvertOrderAction(oa);
                    reversed = true;


                    //leverageOrder.OrderAction = oa;
                }

                // Trader is in the WORST 30%
                // Order to be reversed with variable leverage
                if (glf.GroupContraRating < 0)
                {
                    leverageOrder.OrderAction = EngineUtilities.ConvertOrderAction(oa);
                    reversed = true;
                }

                // Trader is in the 30%-90%
                // Order to be reversed with no leverage change
                if (glf.GroupContraRating == 0)
                {
                    leverageOrder.Quantity = baseLeverage;
                    leverageOrder.OrderAction = EngineUtilities.ConvertOrderAction(oa);
                    reversed = true;
                }

                if (useOverride)
                {
                    leverageOrder.Quantity = baseLeverage;
                    leverageOrder.OrderAction = EngineUtilities.ConvertOrderAction(oa);
                    reversed = true;
                    csw.LogRed(2, "-- OVERRIDE Leverage Enabled: " + baseLeverage + "  base leverage being applied ");

                }


                csw.Log(2, "-- Addtl Leverage Eligible: " + validTrades + "  in the past " + tradeWindow + " days ");
                csw.Log(2, "-- Current PNL: " + lastPNL + "  current MA " + lastAve + "  --  " + movLeverage + " x base applied");
                csw.Log(2, "-- Rank: " + glf.GroupRank + " Grp Count: " + glf.GroupCount + " Percentile: " + glf.GroupPercentage + " -- " + grpLeverage + " x base applied");


            }
            else
            {
                csw.LogRed(2, "Addtl Lev NOT Eligible: " + validTrades + "  in the past " + tradeWindow + " days ");
                leverageOrder.Quantity = baseLeverage;
                leverageOrder.OrderAction = EngineUtilities.ConvertOrderAction(oa);
            }

            if (reversed)
            {
                csw.LogGreen(2, "-- Contra Order Generated");
            }

            if (baseLeverage == leverageOrder.Quantity)
            {
                csw.LogYellow(2, "-- No Additional Leverage Applied");

            }


            return leverageOrder;

        }


        public OrderData CalculateNoContraLeverageQuantity(OrderData LeverageOrder, string orignalTraderID, int orignalOrderDirection, string algoTrader, int baseLeverage, bool useOverride)
        {

            OrderData leverageOrder = LeverageOrder;

            string trader = orignalTraderID;
            leverageOrder.TraderID = algoTrader;
            int oa = orignalOrderDirection;

            leverageOrder.Quantity = baseLeverage;
            leverageOrder.OrderAction = oa;
          
            if (useOverride)
            {
                leverageOrder.Quantity = baseLeverage;
                leverageOrder.OrderAction = EngineUtilities.ConvertOrderAction(oa);
                csw.LogRed(2, "-- OVERRIDE Leverage Enabled: " + baseLeverage + "  leverage being applied ");

            }

            return leverageOrder;

        }



    }
}
