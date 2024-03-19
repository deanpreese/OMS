using OMS.Core.Models;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.Infrastructure.Interfaces;

public interface IUserService
{
    Task<int> AddNewTrader(NewTraderDTO newTrader);
    Task<int> AuthenticateTrader( UserInfoDTO userInfo);
    Task<int> VerifyAndAddByDisplayName( NewTraderDTO newTrader);
}
