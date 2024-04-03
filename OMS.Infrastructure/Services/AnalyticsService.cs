using System;
using System.Threading.Tasks;
using OMS.Application.Interfaces;
using OMS.Application.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Interfaces;
using OMS.SharedKernel.DTO;
using System.Text.Json;
using OMS.Application.Common;
using System.Text.Json.Serialization;

namespace OMS.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private ILogger<AnalyticsService> _logger;
    

    public AnalyticsService(IUnitOfWork unitOfWork, ILogger<AnalyticsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ScoreCard> GetTraderScoreCard(int traderID, int groupID)
    {
        return  await _unitOfWork.AnalyticsRepository.GetTraderScoreCard(traderID, groupID);
    }

    public async Task<ScoreCardDTO> GetTraderScoreCardDTO(int traderID, int groupID)
    {
        ScoreCard scoreCard = await _unitOfWork.AnalyticsRepository.GetTraderScoreCard(traderID, groupID);

        ScoreCardDTO sc_dto = new ScoreCardDTO();
        if (scoreCard != null)
        {
            sc_dto = await DTOMapping.MapScorecardToScorecardDTO(scoreCard);
        }
        return sc_dto;
    }

    public async Task<ScoreCard> UpdateTraderScoreCard(int UserID, int GroupID)
    {
        ScoreCard scoreCard = new ScoreCard
        {
            UserID = UserID,
            GroupID = GroupID
        };
        
        List<ClosedTrade> trades = await _unitOfWork.ClosedOrderRepository.Get_XXX_ClosedOrdersByTrader(UserID, GroupID,-1);

        if(trades.Any())
        {
            scoreCard = TradeStatisticsGenerator.GenerateScoreCard(UserID, GroupID, trades);
            await _unitOfWork.AnalyticsRepository.UpdateTraderScoreCard(scoreCard);
            await _unitOfWork.CommitAsync(); 

            await _unitOfWork.AuditLogRepository.AddToScoreCardLog(scoreCard);
            await _unitOfWork.CommitAsync();
        }

        return  scoreCard;
    }

    public async Task<ScoreCard> UpdateTraderScoreCard(LiveOrder order)
    {
        return  await UpdateTraderScoreCard(order.UserID, order.GroupID);       
    }


    public async Task<int> LogModelOrderData(LiveOrder liveOrder, NewOrderDTO orderDTO, ClosedTradeDTO closedTradeDTO, ScoreCard scoreCard)
    {
        var options = new JsonSerializerOptions {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
            //Converters ={ new JsonStringEnumConverter()},
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        ModelOrderLog modelOrderLog = new ModelOrderLog
        {
            UserID = liveOrder.UserID,
            GroupID = liveOrder.GroupID,
            LiveOrderIDReference = liveOrder.LiveOrderID,
            OrderType = liveOrder.OrderType,
            OrderAction = liveOrder.OrderAction,
            ModelFeatureData = orderDTO.ModelFeatureData,
            LiveOrderJSON = JsonSerializer.Serialize(liveOrder, options),
            ScoreCardJSON = JsonSerializer.Serialize(scoreCard, options),
            ClosedOrderDTOJSON = JsonSerializer.Serialize(closedTradeDTO, options)

        };

        //Console.WriteLine("LogModelOrderData: " + modelOrderLog.LiveOrderJSON);

        await _unitOfWork.AuditLogRepository.AddModelOrderLogEntry(modelOrderLog);
        await _unitOfWork.CommitAsync();

        return  Task.FromResult(0).Result;
    }


}
