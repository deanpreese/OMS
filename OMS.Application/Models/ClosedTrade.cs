using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using OMS.Application.Common;

using OMS.SharedKernel.Common;

namespace OMS.Application.Models;


public class ClosedTrade
{
    private DateTime _openOrderTime;
    private DateTime _closeOrderTime;
    [Key]
    public int StorerID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public string Instrument { get; set; }
    public int Quantity { get; set; }
    public double Leverage { get; set; }
    public bool OppositeTrader { get; set; }
    public int OpenLiveOrderID { get; set; }
    public int OpenPlatformOrderID { get; set; }
    public int OpenOrderMangerID { get; set; }
    public int OpenAuthToken { get; set; }

    // Order ID used to capture ID from Execution 
    public int OpenExecutionID { get; set; }
    // ID used for order matching
    public int OpenRelatedOrderID { get; set; }
    public DateTime OpenOrderTime
    {
        get { return _openOrderTime; }
        set { _openOrderTime = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    public double OpenOrderPX { get; set; }
    public OrderType OpenOrderType { get; set; }
    public OrderAction OpenOrderAction { get; set; }

    // ====================================
    public int ClosePlatformOrderID { get; set; }

    public int ClosedOrderMangerID { get; set; }

    // Trader Token
    public int CloseAuthToken { get; set; }

    // Order ID used to capture ID from Execution 
    public int CloseExecutionID { get; set; }

    // ID used for order matching
    public int CloseRelatedOrderID { get; set; }
    public DateTime CloseOrderTime
    {
        get { return _closeOrderTime; }
        set { _closeOrderTime = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    public double CloseOrderPX { get; set; }
    public OrderType CloseOrderType { get; set; }

    public OrderAction CloseOrderAction { get; set; }

    // ====================================
    public double PNL { get; set; }
    public double MAE { get; set; }
    public double MFE { get; set; }
    public double NetChange { get; set; }

}