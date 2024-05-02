namespace OMS.SharedKernel.DTO;

public class ClosedOrderLogDTO
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
    public long OpenPlatformOrderID { get; set; } // Using long due to the large number
    public int OpenOrderMangerID { get; set; }
    public int OpenAuthToken { get; set; }
    public int OpenExecutionID { get; set; }
    public int OpenRelatedOrderID { get; set; }
    public DateTime OpenOrderTime { get; set; }
    public double OpenOrderPX { get; set; }
    public int OpenOrderType { get; set; }
    public int OpenOrderAction { get; set; }
    public string OpenFeatureData { get; set; }
    public long ClosePlatformOrderID { get; set; } // Using long due to the large number
    public int ClosedOrderMangerID { get; set; }
    public int CloseAuthToken { get; set; }
    public int CloseExecutionID { get; set; }
    public int CloseRelatedOrderID { get; set; }
    public DateTime CloseOrderTime { get; set; }
    public double CloseOrderPX { get; set; }
    public int CloseOrderType { get; set; }
    public int CloseOrderAction { get; set; }
    public string CloseFeatureData { get; set; }
    public double PNL { get; set; }
    public double MAE { get; set; }
    public double MFE { get; set; }
    public double NetChange { get; set; }
}