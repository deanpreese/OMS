using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using OMS.SharedKernel.Common;

namespace OMS.Application.Models;

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
    [Id(9)]
    public string ClosedOrderDTOJSON { get; set; }
}

