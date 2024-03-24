namespace ReplicationTest;

public class ScoreCardJSONData
{
    public string _ct { get; set; }
    public int ScoreCardID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public int Trades { get; set; }
    public int Winners { get; set; }
    public int Losers { get; set; }
    public int Longs { get; set; }
    public int Shorts { get; set; }
    public int NetProfitLong { get; set; }
    public int NetProfitShort { get; set; }
    public int GrossProfit { get; set; }
    public int GrossLoss { get; set; }
    public int LargestWinner { get; set; }
    public int LargestLoser { get; set; }
    public int LargestWinningStreak { get; set; }
    public int LargestLosingStreak { get; set; }
    public int TotalNetProfit { get; set; }
    public string TradeXML { get; set; }
    public double WinLossRatio { get; set; }
    public double AveWin { get; set; }
    public double AveLoss { get; set; }
    public double AveTradeDuration { get; set; }
    public double AveWinDuration { get; set; }
    public double AveLossDuration { get; set; }
    public double StdDevAllTrades { get; set; }
    public double StdDevWinTrades { get; set; }
    public double StdDevLossTrades { get; set; }
    public double SharpRatio { get; set; }
    public double SortinoRatio { get; set; }
    public int PNL_Last3 { get; set; }
    public int PNL_Last5 { get; set; }
    public int PNL_Last8 { get; set; }
    public int PNL_Last13 { get; set; }
    public int PNL_Last21 { get; set; }
    public int PNL_Last34 { get; set; }
    public string LastUpdate { get; set; }
}
