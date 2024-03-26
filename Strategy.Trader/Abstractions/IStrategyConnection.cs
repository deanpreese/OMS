using OMS.SharedKernel.DTO;

namespace Strategy.Trader;

public interface IStrategyConnection
{
    Task Initialize(StrategyAccount strategyAccount);
    Task Connect();
    Task<StrategyAccount> GetStrategyAccount();
    Task<List<LiveOrderDTO>> GetTraderLiveOrders();
    Task<List<LiveOrderDTO>> GetStrategyLiveOrders();
    Task<ClosedTradeDTO> GetLastClosedTraderTradeByOpenPlatformID(int traderPlatformId);
    Task<ScoreCardDTO> GetTraderScoreCard();
    Task<int> ProcessOrderForStrategy( NewOrderDTO order);

}
