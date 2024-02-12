using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using OMS.Core.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using OMS.Data;

namespace OMS.Services.Data;
 
public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private ILogger<AnalyticsService> _logger;
    

    public AnalyticsService(IUnitOfWork unitOfWork, ILogger<AnalyticsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> UpdateScoreCard(UserInfo userInfo)
    {
        List<ClosedTrade> trades = await _unitOfWork.OrderRepository.Get_XXX_ClosedOrdersByTrader(userInfo.UserID, userInfo.GroupNumber,-1);

        try {

            ScoreCard scoreCard = TradeStatisticsGenerator.GenerateScoreCard(userInfo.UserID, userInfo.GroupNumber, trades);
            if (scoreCard != null)
            {
                ScoreCard sc = await _unitOfWork.AnalyticsRepository.GetTraderScoreCard(userInfo.UserID, userInfo.GroupNumber);

                if (sc == null)
                {
                    await _unitOfWork.AnalyticsRepository.AddTraderScoreCard(scoreCard);
                     _unitOfWork.Commit();  
                }
                else
                {
                    await _unitOfWork.AnalyticsRepository.UpdateTraderScoreCard(scoreCard);
                    _unitOfWork.Commit();  
                    
                }   
            }


        }catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }


        return  Task.FromResult(0).Result;
    }



}
