using OMS.Core.Models;

namespace OMS.Infrastructure.Services.Trading;

public interface IUserService
{
    Task<int> AddNewTrader(NewTrader newTrader);
    Task<int> AuthenticateTrader( UserInfo userInfo);
    Task<int> VerifyAndAddByDisplayName( NewTrader newTrader);
}
