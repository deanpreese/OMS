using OMS.Core.Models;

namespace OMS.Infrastructure.Services.Data;
public interface IAnalyticsService
{
    Task<int> UpdateScoreCard(LiveOrder order);
}
