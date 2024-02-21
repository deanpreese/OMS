using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;
using Orleans.Streams;


namespace OMS.Grains.Interfaces;

[Alias("IAdminGrain")]
public interface IAdminGrain : IGrainWithStringKey
{
    Task<int> AddNewTrader(NewTrader newTrader);
    Task<int> AuthenticateTrader(UserInfo userInfo);
    Task<List<UserProfile>> GetTraders(int userGroup);
    Task<int> VerifyByDisplayName(NewTrader newTrader);
    Task<int> AuthByDisplayName(NewTrader newTrader);
   
}