using OMS.Core.Models;
using OMS.Core.Interfaces;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.SharedKernel.Grains;
using Microsoft.Extensions.DependencyInjection;

namespace OMS.Infrastructure.Grains;

public class AdminGrain : Grain, IAdminGrain
{
    private IUnitOfWork _unitOfWork;
    private readonly IServiceScopeFactory _scopeFactory;


    public AdminGrain(IUnitOfWork unitOfWork, IServiceScopeFactory scopeFactory)
    {
        _unitOfWork = unitOfWork;
        _scopeFactory = scopeFactory;
    }


    public async Task<int> AddNewTrader(NewTraderDTO newTrader)
    {
            int traderID = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            await _unitOfWork.CommitAsync();
        return traderID;
    }

    public async Task<int> AddScoreCardForTrader(NewTraderDTO newTrader)
    {
            int traderID = await _unitOfWork.AnalyticsRepository.AddNewTraderScorecard(newTrader.UserID, newTrader.GroupID);
            await _unitOfWork.CommitAsync();
        return traderID;
    }


    public async Task<int> AuthenticateTrader(UserInfoDTO userInfo)
    {
        return await _unitOfWork.TraderRepository.AuthenticateTraderAsync(userInfo.UserID, "abc", userInfo.GroupID);
    }

    public async Task<int> AuthByDisplayName(NewTraderDTO newTrader)
    {
        return await _unitOfWork.TraderRepository.VerifyModelTrader(newTrader);
    }




}
