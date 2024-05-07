using OMS.SharedKernel.DTO;

namespace OMS.SharedKernel.Common;


public interface IStrategyConnection
{
    Task Initialize(StrategyAccount strategyAccount);
    string GetStrategyProfileKey();
    public StrategyAccount CurrentStrategyAccount { get; set; }
    public ScoreCardDTO ModelTraderScoreCardDTO { get; set; }
    public string ModelTraderScoreCardJSON { get; set; }
    public LiveOrderDTO ModelTraderLiveOrderDTO { get; set; }
    public string  ModelTraderLiveOrderJSON { get; set; }
    public ClosedTradeDTO  ModelTraderLastClosedTradeDTO { get; set; }
    public string ModelTraderClosedTradeJSON { get; set; }
    public ModelOrderLogDTO CurrentModelOrderLogDTO { get; set; }

    Task<List<LiveOrderDTO>> GetStrategyLiveOrders();
    Task<ScoreCardDTO> GetTraderScoreCard();

    Task<ClosedTradeDTO> RefreshLastClosedTraderTradeByOpenPlatformID(string trader_key, int traderPlatformId);
    Task<ClosedTradeDTO> GetLastClosedTraderTradeByOpenPlatformID(string trader_key, int traderPlatformId);


    Task OnTraderModelData(ModelOrderLogDTO modelOrderLogDTO);
    Task<int> ProcessOrderForStrategy( NewOrderDTO order);

}
