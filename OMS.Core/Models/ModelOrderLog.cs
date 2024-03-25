using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using OMS.SharedKernel.Common;

namespace OMS.Core.Models;

[GenerateSerializer]
[Alias("ModelOrderLog")]

public class ModelOrderLog
{
     [Key]
    [Id(0)]
    public int ModelOrderLogID { get; set; }
    [Id(1)]
    public int UserID { get; set; }
    [Id(2)]
    public int GroupID { get; set; }
    [Id(3)]
    public int LiveOrderIDReference { get; set; }
    [Id(4)]
    public OrderType OrderType { get; set; }
    [Id(5)]
    public OrderAction OrderAction { get; set; }
    [Id(6)]
    public string LiveOrderJSON { get; set; }
    [Id(7)]
    public string ModelFeatureData { get; set; }
    [Id(8)]
    public string ScoreCardJSON { get; set; }
}


/*
public class ModelOrderLog
{
    [Id(0)]
    private DateTime _createdDate;
    [Id(1)]
    private DateTime _orderTime;

    [Id(17)]
    public int LiveOrderIDReference { get; set; }

    [Key]
    [Id(2)]
    public int ModelOrderLogID { get; set; }
    [Id(3)]
    public int OrderManagerID { get; set; }
    [Id(4)]
    public int UserID { get; set; }
    [Id(5)]
    public int GroupID { get; set; }
    public DateTime DateCreated
    {
        get => _createdDate;
        set => _createdDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    [Id(6)]
    public int PlatformOrderID { get; set; }
    [Id(7)]
    public int RelatedOrderID { get; set; }

    [Id(8)]
    public string Instrument { get; set; } = PlatformConstants.InvalidSymbol;
    [Id(9)]
    public double OrderPX { get; set; }
    [Id(10)]
    public OrderType OrderType { get; set; }
    [Id(11)]
    public OrderAction OrderAction { get; set; }
    [Id(12)]
    public int Quantity { get; set; }
    [Id(13)]
    public double Leverage { get; set; }
    [Id(14)]
    public int Opposite { get; set; }    

    public DateTime OrderTime
    {
        get => _orderTime;
        set => _orderTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    [Id(15)]
    public string ModelFeatureData { get; set; }

    [Id(16)]
    public string ScoreCardJSON { get; set; }


}
*/