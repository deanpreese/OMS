using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;
using Orleans.Streams;


namespace OMS.Grains.Interfaces;

[Alias("ITraderGrain")]
public interface ITraderGrain : IGrainWithStringKey
{
    Task Update(string profile_key);
    Task SetProfileAsync(UserProfile profile_to_set);
    Task SetScoreCardAsync(ScoreCard scoreCard_to_set);
    Task<UserProfile> GetProfileAsync();
    Task<ScoreCard> GetScoreCardAsync();
    Task UpdateProfile(string profile_key);
    Task UpdateScoreCard(string profile_key);

}
