using OMS.Core.Models;

namespace OMS.CQRS.Command;

public class AddNewTraderCommand 
{
    public NewTrader Trader { get; set; }
}
