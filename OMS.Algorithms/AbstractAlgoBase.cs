using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;


using OMS.Core.Models;
using OMS.Core.Logging;

namespace OMS.Algorithms;

public class NewOrderAlgoInfo
{
    public int algoTraderId {get;set;}
    public int newOrderAction {get;set;}
    public int quantity {get;set;}
}


public abstract class AbstractAlgoBase
{
    public bool IsBackTesting {get;set;}    
    public bool UsingDebugging {get;set;}
    public int AlgoGroupNumber {get;set;}
    public string AlgoTraderLong {get;set;}
    public string AlgoTraderShort {get;set;}

    /*
    public DarwinLogger dwl;
    public AlgoOrderManager aom;
    public HistOrderHelper hoh;
    public LiveOrderHelper loh;
    public Authenticator authenticator ;
    public TickHelper th ;
    public AlgoXRefHelper axref ;
    public ClosedPositonXrefHelper cxrefh;
    */
    public int traderIdLong ;
    public int traderIdShort ;

   


    // ----------------------------------------------------------------------
    public void ProcessNewOpeningPosition(OrderFlow order, int inAlgoFilter)
    {

        // Create new opening order
        NewOrderAlgoInfo newOrderInfo = GetOpeningOrderInfo(order, inAlgoFilter);
        //dwl.Information("Processing Opening Order" + order.TraderId + " OA: " + order.OrderAction + " SYM: " + order.Instrument + " LB: "  + newOrderInfo.leaderBoardStatus);

        if (inAlgoFilter != 0 )
        {
            //dwl.Information("Creating New Algo Open Position: Trader " + order.TraderId + " Exe " + order.ExecutionId + " OA " + order.OrderAction + "  IA " + inAlgoFilter);

            if (inAlgoFilter > 0)
            {
                Console.WriteLine("Creating New Algo Open Position: Trader " + order.UserID + " Exe " + order.ExecutedOrderID + " OA " + order.OrderAction + "  IA " + inAlgoFilter);
            }else
            {
                Console.WriteLine("Creating New Algo Open Position: Trader " + order.UserID + " Exe " + order.ExecutedOrderID+ " OA " + order.OrderAction + "  IA " + inAlgoFilter);
            }
            
            /*
            // Convert 
            ActiveTrade trade = ConvertOrder(order, newOrderInfo);
            trade.OrderType = 1;
            // Add new order

            trade.Quantity = order.Quantity  * Math.Abs(inAlgoFilter);

            ActiveTrade newOpeningTrade = aom.ProcessTrade(trade, IsBackTesting);

            if( newOpeningTrade.ExecutionId == 0)
            {
                dwl.Error("Error Opening Algo Position " +  newOpeningTrade.TraderId + "  " + newOpeningTrade.OrderAction);
            }else
            {
                try {
                    AlgoXref xref = new AlgoXref
                    {
                        Instrument = order.Instrument,
                        TraderExecutionId = order.ExecutionId,
                        TraderId = order.TraderId,
                        TraderGroupNumber = order.GroupNumber,
                        TraderOrderQty = order.Quantity,
                        TraderOrderAction = order.OrderAction,
                        AlgoExecutionId = newOpeningTrade.ExecutionId,
                        AlgoId = newOpeningTrade.TraderId,
                        AlgoGroupNumber = newOpeningTrade.GroupNumber,
                        AlgoOrderQty = newOpeningTrade.Quantity,
                        AlgoOrderAction = newOpeningTrade.OrderAction
                    };
                    axref.UpdateXRef(xref);

                } catch (Exception e)
                {
                    dwl.Error( e.ToString() );
                    
                    Console.WriteLine("ERROR OPENING-- Press any key to continue");
                    Console.ReadLine();
                    throw new Exception("Error -" + e);
                }

            }
            */
        }
    }

    // ----------------------------------------------------------------------
    public void ProcessClosingPosition(OrderFlow order)
    {

        /*
        List<ClosedTrade> closedTrades = hoh.GetOrderByClosedExecution(order.ExecutionId , order.GroupNumber);
        if(closedTrades.Count > 0 )
        {
            // This approach assumes that there are only 2 potential XREFs per trader/instrument combination
            // 1 for long entries
            // 1 for short entries
            List<AlgoXref> xrefs = axref.GetXRefs(order.Instrument, order.TraderId, AlgoGroupNumber).Where(x=>x.AlgoOrderQty > 0).ToList();;
            // take the xref that has a quantity attached.  This should be the one that needs closed.                                        

            // if count is ZERO there is nothing to close
            if (xrefs.Count > 0)
            {
                foreach(var x in xrefs)
                {
                    dwl.InfoYellow(" -----------                ------------ ");
                    dwl.InfoYellow("Closing XREF: " + x.Instrument + " Trader: " + x.TraderExecutionId + "  " + x.TraderGroupNumber + " Algo: " +x.AlgoExecutionId + "  " + x.AlgoGroupNumber);
                    dwl.InfoYellow(" -----------                ------------ ");
                }

                AlgoXref xref = xrefs.FirstOrDefault();

                NewOrderAlgoInfo noa = new NewOrderAlgoInfo();
                noa.algoTraderId = xref.AlgoId;

                noa.newOrderAction = 2;
                if ( xref.AlgoOrderAction > 0 )
                {
                    noa.newOrderAction = -1;   
                }

                dwl.InfoRed("Matched Algo Closed Positions " + closedTrades.Count + "  " + order.TraderId + "  " + order.ExecutionId + " " + order.OrderAction);
                ActiveTrade trade = ConvertOrder(order,noa);
                trade.OrderType = -1;

                trade.Quantity = xref.AlgoOrderQty;

                ActiveTrade newClosedTrade = aom.ProcessTrade(trade, IsBackTesting);
                if( newClosedTrade.ExecutionId == 0)
                {
                    dwl.Error("Error Closing Algo Position " +  newClosedTrade.TraderId + "  " + newClosedTrade.OrderAction);
                }else
                {
                    try {
                        StoreClosedPositionXref(closedTrades.FirstOrDefault(), xref, newClosedTrade );
                        axref.ZeroXRef(order.Instrument, order.TraderId, noa.algoTraderId);
                    }catch (Exception ex)
                    {
                        dwl.Error( ex.ToString() );
                        Console.WriteLine("ERROR CLOSING-- Press any key to continue");
                        Console.ReadLine();
                        throw new Exception("Error -" + ex);
                    }
                }
            }
        }//closedTrades.Count
        */
    }

    /*
    // ----------------------------------------------------------------------
    private void StoreClosedPositionXref(ClosedTrade traderClosedTrade, AlgoXref origXref, ActiveTrade algoClosedTrade )
    {
        
        ClosedPositonXref cxref = new ClosedPositonXref();
        cxref.AlgoGroupNumber = origXref.AlgoGroupNumber;
        cxref.AlgoId = origXref.AlgoId;
        cxref.AlgoOrderQty = origXref.AlgoOrderQty;
        cxref.CloseAlgoExecutionId = algoClosedTrade.ExecutionId;
        cxref.CloseTraderExecutionId = traderClosedTrade.CloseExecutionId;
        cxref.OpenAlgoExecutionId = origXref.AlgoExecutionId;
        cxref.Instrument = origXref.Instrument;
        cxref.OpenAlgoOrderAction = origXref.AlgoOrderAction;
        cxref.OpenTraderExecutionId = origXref.TraderExecutionId;
        cxref.OpenTraderOrderAction = origXref.TraderOrderAction;
        cxref.TraderGroupNumber = origXref.TraderGroupNumber;
        cxref.TraderId = origXref.TraderId;
        cxref.TraderOrderQty = origXref.TraderOrderQty;
        cxrefh.AddClosedPositionXref(cxref);   
        
    }
    */


    // ----------------------------------------------------------------------
    private NewOrderAlgoInfo GetOpeningOrderInfo(OrderFlow order, int leaderStatus)
    {
        NewOrderAlgoInfo ai = new NewOrderAlgoInfo();

        // Follow BUY Entry  --  BUY, New Open Long Postion
        if ( leaderStatus > 0 && order.OrderAction > 0 && order.OrderType > 0) { ai.newOrderAction = 2;  ai.algoTraderId = traderIdLong;  }

        // Follow SELL Entry -- SELL, New Open Short Position
        if ( leaderStatus > 0 && order.OrderAction < 0 && order.OrderType > 0) { ai.newOrderAction = -1;  ai.algoTraderId = traderIdShort;  }

        // Fade BUY Entry -- SELL, New Open Short
        if ( leaderStatus < 0 && order.OrderAction > 0 && order.OrderType > 0) { ai.newOrderAction = -1;  ai.algoTraderId = traderIdShort;  }

        // Fade SELL Entry -- BUY, New Long Position
        if ( leaderStatus < 0 && order.OrderAction < 0 && order.OrderType > 0) { ai.newOrderAction = 2;  ai.algoTraderId = traderIdLong;  }

        return ai;
    }



    private double GetLivePrice(OrderFlow ots, NewOrderAlgoInfo newOrderInfo)
    {
        double orderPx = 0.0;

        /*
        LastTick lastTick = th.GetTick(ots.Instrument);

        if ( newOrderInfo.newOrderAction > 0 )
        {
            orderPx = Convert.ToDouble(lastTick.Ask);
        }

        if( newOrderInfo.newOrderAction < 0 )
        {
            orderPx = Convert.ToDouble(lastTick.Bid);
        } 
        */
        return orderPx;
    }

    private double GetBackTestPrice(OrderFlow ots, NewOrderAlgoInfo newOrderInfo)
    {
        double orderPx = 0.0;
        double euroSpread = 0.0002;
        double yenSpread = 0.02;


        /*
        // Buy position/*
        // New order same drection as original -> use price provided
        if((ots.OrderAction > 0 &&  newOrderInfo.newOrderAction > 0) || (ots.OrderAction < 0 &&  newOrderInfo.newOrderAction < 0))
        {
            orderPx = ots.OrderPx;

        }else{

            // orig 1 -- buying at the ASK
            // new -1  -->  rev position -- selling at the BID -- need to subtract spread
            if(ots.OrderAction > newOrderInfo.newOrderAction )
            {
                if ( ots.Instrument.Contains("JPY"))
                {
                    orderPx = ots.OrderPx - yenSpread;                         
                }else
                {
                    orderPx = ots.OrderPx - euroSpread;                         
                }
            }

            // orig -1 -- selling at the BID price
            // new 1  --  rev position -- buying at the ASK -- need to add spread
            if(ots.OrderAction < newOrderInfo.newOrderAction )
            {
                if ( ots.Instrument.Contains("JPY"))
                {
                    orderPx = ots.OrderPx + yenSpread;                         
                }else
                {
                    orderPx = ots.OrderPx + euroSpread;                         
                }
            }
            

        }


        if( ots.OrderPx == orderPx)
        {
            dwl.InfoCyan("Backtesting PRICE MATCH -- Orig OA " + ots.OrderAction + " Orig PX " + ots.OrderPx + "  - New OA " + newOrderInfo.newOrderAction + " New PX " + orderPx);
        }else
        {
            dwl.InfoRed("Backtesting NEW PRICE-- Orig OA " + ots.OrderAction + " Orig PX " + ots.OrderPx + "  - New OA " + newOrderInfo.newOrderAction + " New PX " + orderPx);
        }
        */
        return orderPx;
    }


}
