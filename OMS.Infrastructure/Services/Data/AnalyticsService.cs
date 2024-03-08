using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;
using OMS.Core.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using OMS.Infrastructure.Data.Common;

namespace OMS.Infrastructure.Services.Data;

public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private ILogger<AnalyticsService> _logger;
    

    public AnalyticsService(IUnitOfWork unitOfWork, ILogger<AnalyticsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> UpdateScoreCard(LiveOrder order)
    {
        List<ClosedTrade> trades = await _unitOfWork.OrderRepository.Get_XXX_ClosedOrdersByTrader(order.UserID, order.GroupID,-1);
        ClosedTrade lastClosed = trades.Find(x => x.ClosePlatformOrderID == order.PlatformOrderID);

        while(trades.Count == 0 ||  lastClosed == null)    
        {
            trades = await _unitOfWork.OrderRepository.Get_XXX_ClosedOrdersByTrader(order.UserID, order.GroupID,-1);
            lastClosed = trades.Find(x => x.ClosePlatformOrderID == order.PlatformOrderID);
        }

        try {

            ScoreCard scoreCard = TradeStatisticsGenerator.GenerateScoreCard(order.UserID, order.GroupID, trades);


            if (scoreCard != null)
            {
                ScoreCard sc = await _unitOfWork.AnalyticsRepository.GetTraderScoreCard(order.UserID, order.GroupID);

                if (sc == null)
                {
                    await _unitOfWork.AnalyticsRepository.AddTraderScoreCard(scoreCard);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    await _unitOfWork.AnalyticsRepository.UpdateTraderScoreCard(scoreCard);
                    await _unitOfWork.CommitAsync(); 
                }   
            }else
            {
                Console.WriteLine("No trades found for user " + order.UserID);
            }


        }catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }


        return  Task.FromResult(0).Result;
    }



}
