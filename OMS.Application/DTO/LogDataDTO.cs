using OMS.Application.Models;
using OMS.SharedKernel.DTO;

namespace OMS.Application;

public class LogDataDTO
{
    public LiveOrder liveOrder { get; set; }
    public NewOrderDTO newOrderDTO { get; set; }
    public ClosedTradeDTO closedTradeDTO { get; set; }
}
