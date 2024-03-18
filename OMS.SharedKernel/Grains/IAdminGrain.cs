
using OMS.SharedKernel.DTO;
using Orleans;

namespace OMS.Core.Interfaces;

[Alias("IAdminGrain")]
public interface IAdminGrain : IGrainWithStringKey
{
    Task<int> AddNewTrader(NewTraderDTO newTrader);
    Task<int> AuthenticateTrader(UserInfoDTO userInfo);
    
    Task<int> VerifyAndAddByDisplayName(NewTraderDTO newTrader);
    Task<int> AuthByDisplayName(NewTraderDTO newTrader);

}