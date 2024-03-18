using OMS.Core.Models;
using OMS.Core.Interfaces;

namespace OMS.Infrastructure.Grains;


public class AdminGrain : Grain, IAdminGrain
{
    private IUnitOfWork _unitOfWork;


    public AdminGrain(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<int> AddNewTrader(NewTrader newTrader)
    {
            int traderID = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            await _unitOfWork.CommitAsync();

            traderID = await _unitOfWork.AnalyticsRepository.AddNewTraderScorecard(traderID, newTrader.GroupID);  
            
            await _unitOfWork.CommitAsync();

        return traderID;
    }

    public async Task<int> AuthenticateTrader(UserInfo userInfo)
    {
        return await _unitOfWork.TraderRepository.AuthenticateTraderAsync(userInfo.UserID, "abc", userInfo.GroupID);
    }

    public async Task<List<UserProfile>> GetTraders(int userGroup)
    {
        return await _unitOfWork.TraderRepository.GetUserProfileListAsync(userGroup);
    }

    public async Task<int> VerifyAndAddByDisplayName(NewTrader newTrader)
    {
        int t_v = await _unitOfWork.TraderRepository.VerifyModelTrader(newTrader);
        if (t_v == 0)
        {
            t_v = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            await _unitOfWork.CommitAsync();

            t_v = await _unitOfWork.AnalyticsRepository.AddNewTraderScorecard(t_v, newTrader.GroupID);  
            await _unitOfWork.CommitAsync();

        }
        return t_v;
    }


    public async Task<int> AuthByDisplayName(NewTrader newTrader)
    {
        return await _unitOfWork.TraderRepository.VerifyModelTrader(newTrader);
    }

}
