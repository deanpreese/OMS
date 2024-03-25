using OMS.Application.Models;
using OMS.SharedKernel.DTO;

namespace OMS.Infrastructure.Interfaces;

public interface IAnalyticsService
{
    Task<int> UpdateTraderScoreCard(LiveOrder order);
    
    Task<int> AddNewTraderScoreCard(int traderID, int groupID);

    Task<int> LogModelOrderData(LiveOrder liveOrder, NewOrderDTO orderDTO);
}
