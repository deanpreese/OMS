namespace Strategy.SharedKernel;

public class ScoreCardLogDTO
{
    public string _ct { get; set; }
    public int ScoreCardID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public int Rank { get; set; }
    public int Trades { get; set; }
    public int Winners { get; set; }
    public int Losers { get; set; }
    public int Longs { get; set; }
    public int Shorts { get; set; }
    public decimal NetProfitLong { get; set; }
    public decimal NetProfitShort { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal GrossLoss { get; set; }
    public decimal LargestWinner { get; set; }
    public decimal LargestLoser { get; set; }
    public int LargestWinningStreak { get; set; }
    public int LargestLosingStreak { get; set; }
    public decimal TotalNetProfit { get; set; }
    public decimal WinLossRatio { get; set; }
    public decimal AveWin { get; set; }
    public decimal AveLoss { get; set; }
    public decimal AveTradeDuration { get; set; }
    public decimal AveWinDuration { get; set; }
    public decimal AveLossDuration { get; set; }
    public decimal StdDevAllTrades { get; set; }
    public decimal StdDevWinTrades { get; set; }
    public decimal StdDevLossTrades { get; set; }
    public decimal SharpRatio { get; set; }
    public decimal SortinoRatio { get; set; }
    public decimal PNL_Last3 { get; set; }
    public decimal PNL_Last5 { get; set; }
    public decimal PNL_Last8 { get; set; }
    public decimal PNL_Last13 { get; set; }
    public decimal PNL_Last21 { get; set; }
    public decimal PNL_Last34 { get; set; }
    public DateTime LastUpdate { get; set; }


}

