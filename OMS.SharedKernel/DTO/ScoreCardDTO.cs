using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace OMS.SharedKernel.DTO;

public class ScoreCardDTO
{
    private DateTime _lastUpdate;
    public int ScoreCardID {get; set;}

    public int UserID {get; set;}
    public int GroupID {get; set;}
    public int Rank { get; set; }
    public int SortinoRank { get; set; }
    public int SharpeRank { get; set; }

    public int Trades {get; set;}
    public int Winners  {get; set;}
    public int Losers {get; set;}
    public int Longs {get; set;}
    public int Shorts {get; set;}
    public double NetProfitLong {get; set;}= 0.0;
    public double NetProfitShort {get; set;}= 0.0;
    public double GrossProfit {get; set;}= 0.0;
    public double GrossLoss {get; set;}= 0.0;
    public double LargestWinner {get; set;}= 0.0;
    public double LargestLoser {get; set;} = 0.0;
    public int LargestWinningStreak {get; set;}= 0;
    public int LargestLosingStreak {get; set;} = 0;
    public double TotalNetProfit {get; set;} = 0.0;
    public string TradeXML {get; set;} = "";
    public double WinLossRatio {get; set;} = 0.0;
    public double AveWin {get; set;} = 0.0;
    public double AveLoss {get; set;} = 0.0;
    public double AveTradeDuration {get; set;} = 0.0;
    public double AveWinDuration {get; set;} = 0.0;
    public double AveLossDuration {get; set;} = 0.0;
    public double StdDevAllTrades {get; set;} = 0.0;
    public double StdDevWinTrades {get; set;} = 0.0;
    public double StdDevLossTrades {get; set;} = 0.0;
    public double SharpRatio {get; set;}  = 0.0;
    public double SortinoRatio {get; set;} = 0.0;
    public double PNL_Last3 {get; set;}   = 0.0;
    public double PNL_Last5 {get; set;}  = 0.0;
    public double PNL_Last8 {get; set;}  = 0.0;
    public double PNL_Last13 {get; set;}   = 0.0;
    public double PNL_Last21 {get; set;}  = 0.0;
    public double PNL_Last34 {get; set;} = 0.0;

    public DateTime LastUpdate
    {
        get => _lastUpdate;
        set => _lastUpdate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

}

