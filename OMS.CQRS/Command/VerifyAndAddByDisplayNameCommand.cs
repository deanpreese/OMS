using OMS.Core.Models;

namespace OMS.CQRS;

public class VerifyAndAddByDisplayNameCommand
{
    public NewTrader Trader { get; set; }
}
