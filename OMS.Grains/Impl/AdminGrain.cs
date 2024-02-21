using Orleans;
using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using OMS.Grains.Interfaces;
using Orleans.Runtime;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orleans.Core;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace OMS.Grains.Impl;


public class AdminGrain : Grain, IAdminGrain
{
    private IUnitOfWork _unitOfWork;


    public AdminGrain(IUnitOfWork unitOfWork )
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<int> AddNewTrader(NewTrader newTrader)
    {
        int t_id = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
        _unitOfWork.Commit();
        return t_id;
    }

    public async Task<int> AuthenticateTrader(UserInfo userInfo)
    {
        return await _unitOfWork.TraderRepository.AuthenticateTraderAsync(userInfo.UserID, "abc", userInfo.GroupNumber);
    }

    public async Task<List<UserProfile>> GetTraders(int userGroup)
    {
        return await _unitOfWork.TraderRepository.GetUserProfileListAsync(userGroup);
    }

    public async Task<int> VerifyByDisplayName(NewTrader newTrader)
    {
        int t_v = await _unitOfWork.TraderRepository.VerifyModelTrader(newTrader);
        if( t_v == 0 )
        {
            t_v = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            await _unitOfWork.CommitAsync();
        }
        return t_v;
    }


    public async Task<int> AuthByDisplayName(NewTrader newTrader)
    {
        return await _unitOfWork.TraderRepository.VerifyModelTrader(newTrader);      
    }

}
