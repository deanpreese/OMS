using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using OMS.SharedKernel.Common;

namespace OMS.Application.Models;

public class ModelOrderLog
{
    [Key]
    public int ModelOrderLogID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public int LiveOrderIDReference { get; set; }
    public OrderType OrderType { get; set; }
    public OrderAction OrderAction { get; set; }
    public string LiveOrderJSON { get; set; }
    public string ModelFeatureData { get; set; }
    public string ScoreCardJSON { get; set; }
    public string ClosedOrderDTOJSON { get; set; }
}

