using OMS.Core.Models;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;


namespace OMS.Infrastructure.Services.Trading;

public interface ITradingService
{
    Task<LiveOrder> ProcessNewOrderAsync(NewOrderDTO newOrder);
}
