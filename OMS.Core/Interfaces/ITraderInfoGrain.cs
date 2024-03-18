using OMS.Core.Models;

using OMS.SharedKernel.DTO;

namespace OMS.Core.Interfaces;

[Alias("ITraderInfoGrain")]
public interface ITraderInfoGrain : IGrainWithStringKey
{
    Task<ClosedTrade> GetLastClosedTradeByOpenPlatformID(string profile_key, int platform_id);

    Task<ScoreCardDTO> GetScoreCardAsync(string profile_key);
}
