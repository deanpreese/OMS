using OMS.Core.Models;

namespace OMS.Infrastructure.Interfaces;

public interface IAnalyticsService
{
    Task<int> UpdateScoreCard(LiveOrder order);
    
    Task<int> AddNewTraderScoreCard(int traderID, int groupID);
}
