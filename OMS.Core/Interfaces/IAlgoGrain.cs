
using OMS.Core.Models;

namespace OMS.Core.Interfaces;


[Alias("IAlgoGrain")]
public interface IAlgoGrain : IGrainWithStringKey
{
    Task<int> Algo_ID();
    Task<int> Algo_Group_Number();
    Task<int> GetLiveOrderCount();
    Task<List<LiveOrder>> GetLiveOrders(string profile_key);
    Task<ClosedTrade> GetLastClosedTrade(string profile_key);

}
