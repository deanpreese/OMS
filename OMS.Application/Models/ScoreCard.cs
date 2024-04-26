using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace OMS.Application.Models;

public class ScoreCard
{
    private DateTime _lastUpdate;
    [Key]
    public int ScoreCardID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public int Rank { get; set; }
    public int SortinoRank { get; set; }
    public int SharpeRank { get; set; }
    public int Trades { get; set; }
    public int Winners { get; set; }
    public int Losers { get; set; }
    public int Longs { get; set; }
    public int Shorts { get; set; }
    public double NetProfitLong { get; set; }
    public double NetProfitShort { get; set; }
    public double GrossProfit { get; set; }
    public double GrossLoss { get; set; }
    public double LargestWinner { get; set; }
    public double LargestLoser { get; set; }
    public int LargestWinningStreak { get; set; }
    public int LargestLosingStreak { get; set; }
    public double TotalNetProfit { get; set; }
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
    public double PNL_Last3 { get; set; }
    public double PNL_Last5 { get; set; }
    public double PNL_Last8 { get; set; }
    public double PNL_Last13 { get; set; }
    public double PNL_Last21 { get; set; }
    public double PNL_Last34 { get; set; }

    public DateTime LastUpdate
    {
        get => _lastUpdate;
        set => _lastUpdate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public UserProfile UserProfile { get; set; }
}

