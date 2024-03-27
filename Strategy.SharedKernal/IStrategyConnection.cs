using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;

namespace Strategy.SharedKernel;

public interface IStrategyConnection
{
    Task Initialize(StrategyAccount strategyAccount);
    Task OnTraderModelData(ModelOrderLogDTO modelOrderLogDTO);
    Task<StrategyAccount> GetStrategyAccount();

    Task<List<LiveOrderDTO>> GetStrategyLiveOrders();
    Task<List<LiveOrderDTO>> RefreshStrategyLiveOrders();

    Task<ScoreCardDTO> GetTraderScoreCard();
    Task<ScoreCardDTO> RefreshTraderScoreCard();

    Task<ClosedTradeDTO> RefreshLastClosedTraderTradeByOpenPlatformID(int traderPlatformId);
    Task<ClosedTradeDTO> GetLastClosedTraderTradeByOpenPlatformID(int traderPlatformId);
    
    Task<int> ProcessOrderForStrategy( NewOrderDTO order);

}
