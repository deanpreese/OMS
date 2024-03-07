using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;


namespace OMS.Core.Interfaces;

[Alias("ITraderGrain")]
public interface ITraderGrain : IGrainWithStringKey
{
    Task<int> User_ID();
    Task<int> Group_Number();
    Task<int> GetLiveOrderCount();
    Task Update(string profile_key);
    Task SetProfileAsync(UserProfile profile_to_set);
    Task SetScoreCardAsync(ScoreCard scoreCard_to_set);
    Task<UserProfile> GetProfileAsync(string profile_key);
    Task<ScoreCard> GetScoreCardAsync(string profile_key);
    Task<UserProfile> UpdateProfile(string profile_key);
    Task<ScoreCard> UpdateScoreCard(string profile_key);
    Task<List<LiveOrder>> GetLiveOrders(string profile_key);
    Task<ClosedTrade> GetLastClosedTrade(string profile_key);
    Task<ClosedTrade> GetLastClosedTradeByOpenPlatformID(string profile_key, int platform_id);

}
