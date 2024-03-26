using OMS.Application.Models;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.Infrastructure.Interfaces;

public interface IUserService
{
    Task<int> AddNewTrader(NewTraderDTO newTrader);
    Task<int> VerifyAndAddByDisplayName( NewTraderDTO newTrader);
    
}
