
using OMS.Core.Models;

namespace OMS.Core.Interfaces;


[Alias("IAlgoGrain")]
public interface IStrategyGrain : IGrainWithStringKey
{
     Task<List<LiveOrder>> GetLiveOrders(string profile_key);
}
