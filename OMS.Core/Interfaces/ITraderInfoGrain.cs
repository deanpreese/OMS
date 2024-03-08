using OMS.Core.Models;

namespace OMS.Core.Interfaces;

[Alias("ITraderInfoGrain")]

public interface ITraderInfoGrain : IGrainWithStringKey
{
    Task<ClosedTrade> GetLastClosedTradeByOpenPlatformID(string profile_key, int platform_id);
}
