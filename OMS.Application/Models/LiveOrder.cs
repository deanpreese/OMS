using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS.Application.Common;

using OMS.SharedKernel.Common;

namespace OMS.Application.Models;
public class LiveOrder
{
    private DateTime _orderTime;
    [Key]
     public int LiveOrderID { get; set; }    
    
    public int PlatformOrderID { get; set; }

    public int ExecutedOrderID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public double Leverage { get; set; }
    public int Opposite { get; set; }

    public int AuthToken { get; set; }

    public int OrderManagerID { get; set; }

    public int RelatedOrderID { get; set; }
    public DateTime OrderTime
    {
        get => _orderTime;
        set => _orderTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    public string Instrument { get; set; } = PlatformConstants.INVALID_SYMBOL;
    public double OrderPX { get; set; }

    public OrderType OrderType { get; set; }

    public OrderAction OrderAction { get; set; }
    public int Quantity { get; set; }

    public double MAE { get; set; }
    public double MFE { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string ModelFeatureData { get; set; }

}
