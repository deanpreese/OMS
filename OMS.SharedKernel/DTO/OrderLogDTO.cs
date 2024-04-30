using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OMS.SharedKernel.DTO;

public class OrderLogDTO
{
    public string _ct { get; set; }
    public int OrderFlowId { get; set; }
    public int PlatformOrderID { get; set; }
    public int ExecutedOrderID { get; set; }
    public int AuthToken { get; set; }
    public int OrderManagerID { get; set; }
    public int Leverage { get; set; }
    public int Opposite { get; set; }
    public int RelatedOrderID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public DateTime OrderTime { get; set;}
    public string Instrument { get; set; }
    public double OrderPX { get; set; }
    public int OrderType { get; set; }
    public int OrderAction { get; set; }
    public int Quantity { get; set; }

    
}