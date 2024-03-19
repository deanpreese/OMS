using OMS.Core.Models;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;


namespace OMS.Infrastructure.Interfaces;

public interface ITradingService
{
    Task<LiveOrder> ProcessNewOrderAsync(NewOrderDTO newOrder);
}
