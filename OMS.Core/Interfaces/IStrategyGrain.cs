
using OMS.Core.Models;
using OMS.SharedKernel.DTO;

namespace OMS.Core.Interfaces;


[Alias("IAlgoGrain")]
public interface IStrategyGrain : IGrainWithStringKey
{
     Task<List<LiveOrderDTO>> GetLiveOrders(string profile_key);
}
