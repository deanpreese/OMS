using Orleans;
using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using OMS.Grains.Interfaces;
using Orleans.Runtime;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orleans.Core;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;


namespace OMS.Grains.Interfaces;


[Alias("IAlgoGrain")]
public interface IAlgoGrain : IGrainWithStringKey
{
    Task<int> Algo_ID();
    Task<int> Algo_Group_Number();
    Task<int> GetLiveOrderCount();
    Task<List<LiveOrder>> GetLiveOrders(string profile_key);
    Task<ClosedTrade> GetLastClosedTrade(string profile_key);

}
