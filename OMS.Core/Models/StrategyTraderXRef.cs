using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using OMS.Core.Common;


namespace OMS.Core.Models;


[GenerateSerializer]
[Alias("StrategyTraderXRef")]

public class StrategyTraderXRef
{
    [Id(0)]
    public string TraderKey { get; set; }
    [Id(1)]
    public string StrategyKey { get; set; }
    [Id(2)]
    public long TraderQuantity { get; set; }
    [Id(3)]
    public long StrategyQuantity { get; set; }
    [Id(4)]
    public string Instrument { get; set; }
    [Id(5)]
    public string OrderType { get; set; }
    [Id(6)]
    public OrderAction TraderOrderAction { get; set; }
    [Id(8)]
    public OrderAction StrategyOrderAction { get; set; }
    [Id(7)]
    public long TraderOpenPlatformOrderID { get; set; }
}

