using OMS.Application.Models;
using OMS.SharedKernel.DTO;

namespace OMS.Infrastructure.Interfaces;

public interface IAnalyticsService
{
    Task<ScoreCard> UpdateTraderScoreCard(LiveOrder order);
    Task<ScoreCard> GetTraderScoreCard(int traderID, int groupID);
    Task<ScoreCardDTO> GetTraderScoreCardDTO(int traderID, int groupID);
    Task<ModelOrderLog> LogModelOrderData(LiveOrder liveOrder, NewOrderDTO orderDTO, ClosedTradeDTO closedTradeDTO, ScoreCard scoreCard);
}
