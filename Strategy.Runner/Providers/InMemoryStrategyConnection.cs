using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.Json;
using OMS.SharedKernel;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;


namespace Strategy.Runner.Provider;

public class InMemoryStrategyConnection : ScreenColorBase,  IStrategyConnection
{
    public StrategyAccount CurrentStrategyAccount { get; set; }
    public ScoreCardDTO ModelTraderScoreCardDTO { get; set; }
    public string ModelTraderScoreCardJSON { get; set; }
    public LiveOrderDTO ModelTraderLiveOrderDTO { get; set; }
    public string  ModelTraderLiveOrderJSON { get; set; }
    public ClosedTradeDTO  ModelTraderLastClosedTradeDTO { get; set; }
    public string ModelTraderClosedTradeJSON { get; set; }
    public ModelOrderLogDTO CurrentModelOrderLogDTO { get; set; }

    CommonApiClient _apiClient;
    
    public InMemoryStrategyConnection()
    {
        _apiClient = new CommonApiClient();
    }

    public string GetStrategyProfileKey()
    {
        return CurrentStrategyAccount.strategy_traderId + "_" + CurrentStrategyAccount.group;    
    }

    public async Task Initialize(StrategyAccount strategyAccountFromJSON)
    {
        CurrentStrategyAccount = strategyAccountFromJSON;

        NewTraderDTO n_strategy = new NewTraderDTO
        {
            UserID = 0,
            DisplayName = CurrentStrategyAccount.strategy_name,
            GroupID = CurrentStrategyAccount.group,
            UserPwd = "abc",
            FirstName = "Algo",
            LastName = CurrentStrategyAccount.strategy_name,
            Email = "abc@abc"
        };
        
        int trader_id = await _apiClient.VerifyAndAddByDisplayNameAsync(n_strategy);
        CurrentStrategyAccount.strategy_traderId = trader_id;
    }
        
    
    public Task OnTraderModelData(ModelOrderLogDTO modelOrderLogDTO)
    {
        CurrentModelOrderLogDTO = modelOrderLogDTO;


        if (modelOrderLogDTO.ScoreCardDeserialized() != null)
        {
            ModelTraderScoreCardDTO = modelOrderLogDTO.ScoreCardDeserialized();
        }

        if (modelOrderLogDTO.LiveOrderDeserialized != null)
        {
            ModelTraderLiveOrderDTO = modelOrderLogDTO.LiveOrderDeserialized;
        }

        if (modelOrderLogDTO.ClosedTradeDeserialized != null)
        {
            ModelTraderLastClosedTradeDTO = modelOrderLogDTO.ClosedTradeDeserialized;
        }

        if (modelOrderLogDTO.ScoreCardJson != null)
        {
            ModelTraderScoreCardJSON = modelOrderLogDTO.ScoreCardJson;
        }

        if (modelOrderLogDTO.LiveOrderJson != null)
        {
            ModelTraderLiveOrderJSON = modelOrderLogDTO.LiveOrderJson;
        }

        if (modelOrderLogDTO.ClosedOrderJson != null)
        {
            ModelTraderClosedTradeJSON = modelOrderLogDTO.ClosedOrderJson;
        }

        return Task.CompletedTask;

    }
    
    public async Task<ClosedTradeDTO> RefreshLastClosedTraderTradeByOpenPlatformID(string trader_key, int traderPlatformId)
    {
        return await _apiClient.GetLastClosedTradeByOpenPlatformIDAsync( trader_key, traderPlatformId);
    }

    public Task<ClosedTradeDTO> GetLastClosedTraderTradeByOpenPlatformID(string trader_key, int traderPlatformIdStrategyRelatedOrderID)
    {
        return  RefreshLastClosedTraderTradeByOpenPlatformID(trader_key, traderPlatformIdStrategyRelatedOrderID);
        
    }

    public async Task<List<LiveOrderDTO>> GetStrategyLiveOrders()
    {
        return await _apiClient.GetLiveOrdersAsync(GetStrategyProfileKey());
    }

    public async Task<ScoreCardDTO> GetTraderScoreCard()
    {
        return await Task.FromResult(ModelTraderScoreCardDTO);
    }

    public async Task<int> ProcessOrderForStrategy(NewOrderDTO order)
    {
        int oid = await _apiClient.ProcessOrderAsync(order);

        string json = JsonSerializer.Serialize(order);
        //await _producer.Send(json);

        switch (order.OrderType)
        {
            case OMS.SharedKernel.Common.OrderType.OPEN:
                Console.WriteLine($"{GREEN}{oid} {CurrentStrategyAccount.strategy_name} ProcessOrderForStrategy: {order.UserID} {order.OrderAction} {order.OrderType}");
                Console.ResetColor();
                break;
            case OMS.SharedKernel.Common.OrderType.CLOSE:
                Console.WriteLine($"{MAGENTA}{oid} {CurrentStrategyAccount.strategy_name} ProcessOrderForStrategy: {order.UserID} {order.OrderAction} {order.OrderType}");
                Console.ResetColor();
                break;

            default:
                Console.WriteLine(oid + " " + CurrentStrategyAccount.strategy_name + " ProcessOrderForStrategy: " + order.UserID + " " + order.OrderAction + " " + order.OrderType);
                break;                            
        }

        return await Task.FromResult(oid);
    }

}
