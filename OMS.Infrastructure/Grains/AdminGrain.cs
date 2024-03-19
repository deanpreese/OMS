using OMS.Core.Models;
using OMS.Core.Interfaces;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.SharedKernel.Grains;



namespace OMS.Infrastructure.Grains;



public class AdminGrain : Grain, IAdminGrain
{
    private IUnitOfWork _unitOfWork;


    public AdminGrain(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<int> AddNewTrader(NewTraderDTO newTrader)
    {
            int traderID = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            await _unitOfWork.CommitAsync();

            traderID = await _unitOfWork.AnalyticsRepository.AddNewTraderScorecard(traderID, newTrader.GroupID);  
            
            await _unitOfWork.CommitAsync();

        return traderID;
    }

    public async Task<int> AuthenticateTrader(UserInfoDTO userInfo)
    {
        return await _unitOfWork.TraderRepository.AuthenticateTraderAsync(userInfo.UserID, "abc", userInfo.GroupID);
    }


    public async Task<int> VerifyAndAddByDisplayName(NewTraderDTO newTrader)
    {
        int t_v = await _unitOfWork.TraderRepository.VerifyModelTrader(newTrader);
        if (t_v == 0)
        {
            await Task.Run(async () =>
            {
                t_v = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
                await _unitOfWork.CommitAsync();

                t_v = await _unitOfWork.AnalyticsRepository.AddNewTraderScorecard(t_v, newTrader.GroupID);  
                await _unitOfWork.CommitAsync();

            });

           

        }
        return t_v;
    }


    public async Task<int> AuthByDisplayName(NewTraderDTO newTrader)
    {
        return await _unitOfWork.TraderRepository.VerifyModelTrader(newTrader);
    }

}
