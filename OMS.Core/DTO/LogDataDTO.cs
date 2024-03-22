using OMS.Core.Models;
using OMS.SharedKernel.DTO;

namespace OMS.Core;

public class LogDataDTO
{
    public LiveOrder liveOrder { get; set; }
    public NewOrderDTO newOrderDTO { get; set; }
}
