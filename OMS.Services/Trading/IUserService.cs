using OMS.Core.Models;

namespace OMS.Services.Trading;

public interface IUserService
{
    Task<int> AddNewTrader(NewTrader newTrader);
    Task<int> AuthenticateTrader( UserInfo userInfo);
    Task<int> VerifyByDisplayName( NewTrader newTrader);
}
