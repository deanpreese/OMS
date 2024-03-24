using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;
using OMS.Core.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using OMS.Infrastructure.Data.Common;
using OMS.Infrastructure.Interfaces;
using OMS.SharedKernel.DTO;
using System.Text.Json;

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

    public async Task<int> AddNewTraderScoreCard(int traderID, int groupID)
    {
        return await _unitOfWork.AnalyticsRepository.AddNewTraderScorecard(traderID, groupID);
    }


    public async Task<int> UpdateTraderScoreCard(int UserID, int GroupID)
    {
        List<ClosedTrade> trades = await _unitOfWork.ClosedOrderRepository.Get_XXX_ClosedOrdersByTrader(UserID, GroupID,-1);
        if(trades.Any())
        {
            ScoreCard scoreCard = TradeStatisticsGenerator.GenerateScoreCard(UserID, GroupID, trades);
            if (scoreCard != null)
            {
                ScoreCard sc = await _unitOfWork.AnalyticsRepository.GetTraderScoreCard(UserID, GroupID);

                if (sc == null)
                {
                    await _unitOfWork.AnalyticsRepository.AddNewTraderScorecard(UserID, GroupID);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    await _unitOfWork.AnalyticsRepository.UpdateTraderScoreCard(scoreCard);
                    await _unitOfWork.CommitAsync(); 
                }   
            }
        }

        return  Task.FromResult(0).Result;
    }

    public async Task<int> UpdateTraderScoreCard(LiveOrder order)
    {
        return  await UpdateTraderScoreCard(order.UserID, order.GroupID);       
    }


    public async Task<int> LogModelOrderData(LiveOrder liveOrder, NewOrderDTO orderDTO)
    {

        ScoreCard sc = await _unitOfWork.AnalyticsRepository.GetTraderScoreCard(liveOrder.UserID, liveOrder.GroupID);
        var options = new JsonSerializerOptions {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

        ModelOrderLog modelOrderLog = new ModelOrderLog
        {
            OrderManagerID = liveOrder.OrderManagerID,
            UserID = liveOrder.UserID,
            GroupID = liveOrder.GroupID,
            DateCreated = DateTime.UtcNow,
            PlatformOrderID = liveOrder.PlatformOrderID,
            RelatedOrderID = liveOrder.RelatedOrderID,
            Instrument = liveOrder.Instrument,
            OrderPX = liveOrder.OrderPX,
            OrderType = liveOrder.OrderType,
            OrderAction = liveOrder.OrderAction,
            Quantity = liveOrder.Quantity,
            Leverage = liveOrder.Leverage,
            Opposite = liveOrder.Opposite,
            OrderTime = liveOrder.OrderTime,
            ModelFeatureData = orderDTO.ModelFeatureData,
            ScoreCardJSON = JsonSerializer.Serialize(sc, options)

        };

        await _unitOfWork.AnalyticsRepository.AddModelOrderLogEntry(modelOrderLog);
        await _unitOfWork.CommitAsync();

        return  Task.FromResult(0).Result;
    }


}
