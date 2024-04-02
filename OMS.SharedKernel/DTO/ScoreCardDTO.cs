using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;


namespace OMS.SharedKernel.DTO;


[GenerateSerializer]
[Alias("ScoreCardDTO")]

public class ScoreCardDTO
{
    [Id(21)]
    private DateTime _lastUpdate;


    [Key]
    [Id(0)]
    public int ScoreCardID {get; set;}


    [Id(36)]
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
    public double NetProfitLong {get; set;}= 0.0;
    [Id(9)]
    public double NetProfitShort {get; set;}= 0.0;
    [Id(10)]
    public double GrossProfit {get; set;}= 0.0;
    [Id(11)]
    public double GrossLoss {get; set;}= 0.0;
    [Id(12)]
    public double LargestWinner {get; set;}= 0.0;
    [Id(13)]
    public double LargestLoser {get; set;} = 0.0;
    [Id(14)]
    public int LargestWinningStreak {get; set;}= 0;
    [Id(15)]
    public int LargestLosingStreak {get; set;} = 0;
    [Id(16)]
    public double TotalNetProfit {get; set;} = 0.0;
    [Id(17)]
    public string TradeXML {get; set;} = "";
    [Id(18)]
    public double WinLossRatio {get; set;} = 0.0;
    [Id(19)]
    public double AveWin {get; set;} = 0.0;
    [Id(20)]
    public double AveLoss {get; set;} = 0.0;

    [Id(22)]
    public double AveTradeDuration {get; set;} = 0.0;
    [Id(23)]
    public double AveWinDuration {get; set;} = 0.0;
    [Id(24)]
    public double AveLossDuration {get; set;} = 0.0;
    [Id(25)]
    public double StdDevAllTrades {get; set;} = 0.0;
    [Id(26)]
    public double StdDevWinTrades {get; set;} = 0.0;
    [Id(27)]
    public double StdDevLossTrades {get; set;} = 0.0;
    [Id(28)]
    public double SharpRatio {get; set;}  = 0.0;
    [Id(29)]
    public double SortinoRatio {get; set;} = 0.0;
    [Id(30)]
    public double PNL_Last3 {get; set;}   = 0.0;
    [Id(31)]
    public double PNL_Last5 {get; set;}  = 0.0;
    [Id(32)]
    public double PNL_Last8 {get; set;}  = 0.0;
    [Id(33)]
    public double PNL_Last13 {get; set;}   = 0.0;
    [Id(34)]
    public double PNL_Last21 {get; set;}  = 0.0;
    [Id(35)]
    public double PNL_Last34 {get; set;} = 0.0;

    public DateTime LastUpdate
    {
        get => _lastUpdate;
        set => _lastUpdate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

}

