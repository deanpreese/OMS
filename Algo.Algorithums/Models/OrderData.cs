
namespace Algo.Algorithums.Models;

public class OrderData
{
    // Order Id from Platform if possible
    public int PlatformOrderID { get; set; }

    // Trader Token
    public int AuthToken { get; set; }
    // Order ID used to capture ID from Execution 
    public int ExecutionID { get; set; }
    // ID used for order matching
    public int RelatedOrderID { get; set; }
    public string? TraderID { get; set; }
    public int GroupNumber = 0;
    public DateTime OrderTime { get; set; }
    public string? Instrument { get; set; }
    public double OrderPX { get; set; }
    public int OrderType { get; set; }
    public int OrderAction { get; set; }
    public int Quantity { get; set; }
    public double Leverage { get; set; }
    public bool OppositeTrader { get; set; }


    // ====================================
    public string InstrumentType()
    {
        string rtn = "NON";
        return rtn;
}

public double PNL { get; set; }
public double MAE { get; set; }
public double MFE { get; set; }
public int RTOrderID { get; set; }


}

