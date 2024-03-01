

namespace Algo.Trader.Models;

public class TraderXRef
{
    public string TraderID { get; set; }
    public long ActualQuantity { get; set; }
    public long LevQuantity { get; set; }
    public string Instrument { get; set; }
    public long RID { get; set; }
    public string AlgoTrader { get; set; }
}

