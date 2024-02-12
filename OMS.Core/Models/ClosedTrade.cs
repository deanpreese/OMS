using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using OMS.Core.Common;


namespace OMS.Core.Models;

[GenerateSerializer]
[Alias("ClosedTrade")]

public class ClosedTrade
{
    [Id(0)]
    private DateTime _openOrderTime;
    [Id(1)]
    private DateTime _closeOrderTime;


    [Key]
    [Id(2)]
    public int StorerID { get; set; }

    [Id(3)]
    public int UserID { get; set; }

    [Id(4)]
    public int GroupID { get; set; }

    [Id(5)]
    public string? Instrument { get; set; }

    [Id(6)]
    public int Quantity { get; set; }

    [Id(7)]
    public double Leverage { get; set; }

    [Id(8)]
    public bool OppositeTrader { get; set; }

    [Id(9)]
    public int OpenPlatformOrderID { get; set; }

    // Trader Token
    [Id(10)]
    public int OpenAuthToken { get; set; }

    // Order ID used to capture ID from Execution 
    [Id(11)]
    public int OpenExecutionID { get; set; }

    // ID used for order matching

    [Id(12)]
    public int OpenRelatedOrderID { get; set; }
    public DateTime OpenOrderTime
    {
        get { return _openOrderTime; }
        set { _openOrderTime = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    [Id(13)]
    public double OpenOrderPX { get; set; }

    [Id(14)]
    public OrderType OpenOrderType { get; set; }

    [Id(15)]
    public OrderAction OpenOrderAction { get; set; }

    // ====================================
    [Id(16)]
    public int ClosePlatformOrderID { get; set; }

    // Trader Token
    [Id(17)]
    public int CloseAuthToken { get; set; }

    // Order ID used to capture ID from Execution 
    [Id(18)]
    public int CloseExecutionID { get; set; }

    // ID used for order matching
    [Id(19)]
    public int CloseRelatedOrderID { get; set; }
    public DateTime CloseOrderTime
    {
        get { return _closeOrderTime; }
        set { _closeOrderTime = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    [Id(20)]
    public double CloseOrderPX { get; set; }

    [Id(21)]
    public OrderType CloseOrderType { get; set; }

    [Id(22)]
    public OrderAction CloseOrderAction { get; set; }

    // ====================================

    [Id(23)]
    public double PNL { get; set; }

    [Id(24)]
    public double MAE { get; set; }

    [Id(25)]
    public double MFE { get; set; }

    [Id(26)]
    public double NetChange { get; set; }


    [Timestamp]
    [Id(27)]

    public byte[]? Version { get; set; }

}