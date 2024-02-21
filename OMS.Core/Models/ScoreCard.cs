using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace OMS.Core.Models;

[GenerateSerializer]
[Alias("ScoreCard")]

public class ScoreCard
{
    [Id(21)]
    private DateTime _lastUpdate;


    [Key]
    [Id(0)]
    public int ScoreCardID {get; set;}
    
    [Id(1)]
    public int UserID {get; set;}
    [Id(2)]
    public int GroupID {get; set;}
    [Id(3)]
    public int Trades {get; set;}
    [Id(4)]
    public int Winners  {get; set;}
    [Id(5)]
    public int Losers {get; set;}
    [Id(6)]
    public int Longs {get; set;}
    [Id(7)]
    public int Shorts {get; set;}
    [Id(8)]
    public double NetProfitLong {get; set;}
    [Id(9)]
    public double NetProfitShort {get; set;}
    [Id(10)]
    public double GrossProfit {get; set;}
    [Id(11)]
    public double GrossLoss {get; set;}
    [Id(12)]
    public double LargestWinner {get; set;}
    [Id(13)]
    public double LargestLoser {get; set;}
    [Id(14)]
    public int LargestWinningStreak {get; set;}
    [Id(15)]
    public int LargestLosingStreak {get; set;}
    [Id(16)]
    public double TotalNetProfit {get; set;}
    [Id(17)]
    public string TradeXML {get; set;} = "";
    [Id(18)]
    public double WinLossRatio {get; set;}
    [Id(19)]
    public double AveWin {get; set;}
    [Id(20)]
    public double AveLoss {get; set;}

    public double AveTradeDuration {get; set;}
    public double AveWinDuration {get; set;}
    public double AveLossDuration {get; set;}
    public double StdDevAllTrades {get; set;}
    public double StdDevWinTrades {get; set;}
    public double StdDevLossTrades {get; set;}
    public double SharpRatio {get; set;}
    public double SortinoRatio {get; set;}

    public double PNL_Last3 {get; set;}
    public double PNL_Last5 {get; set;}
    public double PNL_Last8 {get; set;}
    public double PNL_Last13 {get; set;}
    public double PNL_Last21 {get; set;}
    public double PNL_Last34 {get; set;}

    public DateTime LastUpdate
    {
        get => _lastUpdate;
        set => _lastUpdate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

}

