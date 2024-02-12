using OMS.Core.Models;

namespace OMS.Services.Data;

public interface IAnalyticsService
{
    Task<int> UpdateScoreCard(UserInfo userInfo);
}
