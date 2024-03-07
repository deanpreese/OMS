using OMS.Core.Models;

namespace OMS.Core.Interfaces;

[Alias("IAdminGrain")]
public interface IAdminGrain : IGrainWithStringKey
{
    Task<int> AddNewTrader(NewTrader newTrader);
    Task<int> AuthenticateTrader(UserInfo userInfo);
    Task<List<UserProfile>> GetTraders(int userGroup);
    Task<int> VerifyAndAddByDisplayName(NewTrader newTrader);
    Task<int> AuthByDisplayName(NewTrader newTrader);

}