using OMS.Application.Models;
using Microsoft.Extensions.Logging;
using OMS.Application.Interfaces;
using OMS.Infrastructure.Data;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.Infrastructure.Interfaces;

namespace OMS.Infrastructure.Services;

public class UserService : IUserService
{
    IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> AddNewTrader(NewTraderDTO newTrader)
    {
        int traderID = 0;
        try
        {
            traderID = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            _unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return traderID;
    }

    public async Task<int> VerifyAndAddByDisplayName(NewTraderDTO newTrader)
    {
        int t_v = await _unitOfWork.TraderRepository.VerifyModelTrader(newTrader);

        if (t_v == 0)
        {
            t_v = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            _unitOfWork.Commit();
        }
        return t_v;
    }
}
