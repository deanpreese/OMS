using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMS.Core.Common;

// ***************************************************
public static class InstrumentUtility
{


    public static double CalcNetDollars(string sym, double pnl)
    {
        double rtn = 0;

        TrackedInstruments ti = new TrackedInstruments();
        InstrumentData idata = ti.FindData(sym);
        double total_rtn = pnl * idata.TickValue * idata.PointValue;

        rtn = total_rtn;

        return rtn;
    }

    public static double CalcPNL(string sym, OrderAction closeOrderAction, double entryPX, double exitPX, int qty)
    {
        double PNL = 0;

        TrackedInstruments ti = new TrackedInstruments();
        InstrumentData iData = ti.FindData(sym);

        double minLotSize = iData.MinLotSize;
        double tickValue = iData.TickValue;
        double ptVal = iData.PointValue;

        // Order Action
        //   Buying to Cover  2    Close Short Position
        //  Selling -1      Close Long Position

        if (closeOrderAction == OrderAction.Sell)
        {
            // Long Exit
            try
            {
                PNL = Math.Round(((exitPX - entryPX) / tickValue) * (qty / minLotSize), 2, MidpointRounding.AwayFromZero);
            }
            catch (Exception) { PNL = 9999; }

        }

        if (closeOrderAction == OrderAction.Buy)
        {
            //  Short Exit
            try
            {
                PNL = Math.Round(((entryPX - exitPX) / tickValue) * (qty / minLotSize), 2, MidpointRounding.AwayFromZero);
            }
            catch (Exception) { PNL = 9999; }

        }
        return PNL;
    }


}
// ***************************************************
