using System.ComponentModel.DataAnnotations;
using OMS.Application.Common;

using OMS.SharedKernel.Common;

namespace OMS.Application.Models;

public class OrderLog
{
    private DateTime _orderTime;
    [Key]
    public int OrderFlowId { get; set; }

    public int PlatformOrderID { get; set; }

    public int ExecutedOrderID { get; set; }

    public int AuthToken { get; set; }

    public int OrderManagerID { get; set; }

    public double Leverage { get; set; }

    public int Opposite { get; set; }

    public int RelatedOrderID { get; set; }

    public int UserID { get; set; }

    public int GroupID { get; set; }

    public DateTime OrderTime
    {
        get => _orderTime;
        set => _orderTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public string Instrument { get; set; } = "";

    public double OrderPX { get; set; }

    public OrderType OrderType { get; set; }

    public OrderAction OrderAction { get; set; }

    public int Quantity { get; set; }
}
