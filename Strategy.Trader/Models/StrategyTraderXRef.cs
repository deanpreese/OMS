namespace Strategy.Trader.Models;

public class StrategyTraderXRef
{
    public string TraderKey { get; set; }
    public long TraderQuantity { get; set; }
    public long StrategyQuantity { get; set; }
    public string Instrument { get; set; }
    public long TraderOpenPlatformOrderID { get; set; }
    public string StrategyTrader { get; set; }
}

