using System;

namespace ReplicationTest;

public class ClosedTradeJSON
{
    public string _ct { get; set; }
    public int StorerID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public string Instrument { get; set; }
    public int Quantity { get; set; }
    public int Leverage { get; set; }
    public bool OppositeTrader { get; set; }
    public int OpenLiveOrderID { get; set; }
    public Int64 OpenPlatformOrderID { get; set; }
    public int OpenOrderMangerID { get; set; }
    public int OpenAuthToken { get; set; }
    public int OpenExecutionID { get; set; }
    public int OpenRelatedOrderID { get; set; }
    public string OpenOrderTime { get; set; }
    public double OpenOrderPX { get; set; }
    public int OpenOrderType { get; set; }
    public int OpenOrderAction { get; set; }
    public Int64 ClosePlatformOrderID { get; set; }
    public int ClosedOrderMangerID { get; set; }
    public int CloseAuthToken { get; set; }
    public int CloseExecutionID { get; set; }
    public int CloseRelatedOrderID { get; set; }
    public string CloseOrderTime { get; set; }
    public double CloseOrderPX { get; set; }
    public int CloseOrderType { get; set; }
    public int CloseOrderAction { get; set; }
    public double PNL { get; set; }
    public double MAE { get; set; }
    public double MFE { get; set; }
    public double NetChange { get; set; }

}
