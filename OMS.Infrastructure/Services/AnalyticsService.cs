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


    public async Task<ScoreCardDTO> GetTraderScoreCard(int traderID, int groupID)
    {
        ScoreCard scoreCard = await _unitOfWork.AnalyticsRepository.GetTraderScoreCard(traderID, groupID);

        ScoreCardDTO sc_dto = new ScoreCardDTO();
        if (scoreCard != null)
        {
            sc_dto = await DTOMapping.MapScorecardToScorecardDTO(scoreCard);
        }
        return sc_dto;
    }

    public async Task<int> AddNewTraderScoreCard(int traderID, int groupID)
    {
         traderID = await _unitOfWork.AnalyticsRepository.AddNewTraderScorecard(traderID, groupID);
         _unitOfWork.Commit();
        _logger.LogInformation($"Trader Scorecard added  {traderID} {groupID} ");
        return traderID;        
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


    public async Task<int> LogModelOrderData(LiveOrder liveOrder, NewOrderDTO orderDTO, ClosedTradeDTO closedTradeDTO)
    {

        ScoreCard sc = await _unitOfWork.AnalyticsRepository.GetTraderScoreCard(liveOrder.UserID, liveOrder.GroupID);
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
            LiveOrderJSON = JsonSerializer.Serialize(liveOrder, options),
            ModelFeatureData = orderDTO.ModelFeatureData,
            ScoreCardJSON = JsonSerializer.Serialize(sc, options),
            ClosedOrderDTOJSON = JsonSerializer.Serialize(closedTradeDTO, options)

        };

        await _unitOfWork.AnalyticsRepository.AddModelOrderLogEntry(modelOrderLog);
        await _unitOfWork.CommitAsync();

        return  Task.FromResult(0).Result;
    }


}
