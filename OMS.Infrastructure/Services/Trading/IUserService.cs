using OMS.Core.Models;
using OMS.Core.DTO;


namespace OMS.Infrastructure.Services.Trading;

public interface IUserService
{
    Task<int> AddNewTrader(NewTraderDTO newTrader);
    Task<int> AuthenticateTrader( UserInfoDTO userInfo);
    Task<int> VerifyAndAddByDisplayName( NewTraderDTO newTrader);
}
