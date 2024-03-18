
using OMS.SharedKernel.DTO;

namespace OMS.SharedKernel.Grains;

[Alias("ITraderInfoGrain")]
public interface ITraderInfoGrain : IGrainWithStringKey
{
    Task<ClosedTradeDTO> GetLastClosedTradeByOpenPlatformID(string profile_key, int platform_id);

    Task<ScoreCardDTO> GetScoreCardAsync(string profile_key);
}
