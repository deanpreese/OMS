using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMS.Core.Common;

public class InstrumentData
{
    public InstrumentData()
    {
        Symbol = "NONE";
        MinLotSize = 0;
        TickValue = 0;
        PointValue = 0;
    }

    public InstrumentData(string _Sym, double _PointVal, double _TickVal, double _MinLot)
    {
        Symbol = _Sym;
        MinLotSize = _MinLot;
        TickValue = _TickVal;
        PointValue = _PointVal;
    }

    public string Symbol { get; set; }
    public double MinLotSize { get; set; }
    public double TickValue { get; set; }
    public double PointValue { get; set; }
}