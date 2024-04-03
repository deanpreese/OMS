using System.Drawing;
using System.Runtime.CompilerServices;
using OMS.SharedKernel;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;
using Strategy.Trader.Abstractions;

namespace Strategy.Server.Providers;

public class InMemoryStrategyConnection :  StrategyApiClient, IStrategyConnection
{
    public StrategyAccount CurrentStrategyAccount { get; set; }
    public ScoreCardDTO ModelTraderScoreCardDTO { get; set; }
    public string ModelTraderScoreCardJSON { get; set; }
    public LiveOrderDTO ModelTraderLiveOrderDTO { get; set; }
    public string  ModelTraderLiveOrderJSON { get; set; }
    public ClosedTradeDTO  ModelTraderLastClosedTradeDTO { get; set; }
    public string ModelTraderClosedTradeJSON { get; set; }
    public ModelOrderLogDTO CurrentModelOrderLogDTO { get; set; }


    public InMemoryStrategyConnection(string baseURL)
    {
        base.BaseUrl = baseURL;
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
        
        int trader_id = await VerifyAndAddByDisplayNameAsync(n_strategy);
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
    
    public StrategyAccount GetStrategyAccount()
    {
        return CurrentStrategyAccount ;
    }

    public async Task<ClosedTradeDTO> RefreshLastClosedTraderTradeByOpenPlatformID(string trader_key, int traderPlatformId)
    {
        return await GetLastClosedTradeByOpenPlatformIDAsync( trader_key, traderPlatformId);
    }

    public Task<ClosedTradeDTO> GetLastClosedTraderTradeByOpenPlatformID(string trader_key, int traderPlatformIdStrategyRelatedOrderID)
    {
        /*
        if( ModelTraderLastClosedTradeDTO.OpenPlatformOrderID == traderPlatformIdStrategyRelatedOrderID)
        {
            Console.WriteLine($"{YELLOW}USING Current --- GetLastClosedTraderTradeByOpenPlatformID: " + trader_key + " " +  traderPlatformIdStrategyRelatedOrderID);
            Console.WriteLine(ModelTraderClosedTradeJSON); 
            Console.ResetColor();
            return Task.FromResult(ModelTraderLastClosedTradeDTO);
        }else
        {
            Console.WriteLine($"{MAGENTA}REFRESHING  === GetLastClosedTraderTradeByOpenPlatformID: " + trader_key + " " +  traderPlatformIdStrategyRelatedOrderID);
            Console.WriteLine("Store " +  ModelTraderLastClosedTradeDTO.StorerID + " OP_ID " + ModelTraderLastClosedTradeDTO.OpenPlatformOrderID);
            Console.WriteLine(ModelTraderClosedTradeJSON); 
            Console.ResetColor();
            
            return  RefreshLastClosedTraderTradeByOpenPlatformID(trader_key, traderPlatformIdStrategyRelatedOrderID);
        }
        */
        return  RefreshLastClosedTraderTradeByOpenPlatformID(trader_key, traderPlatformIdStrategyRelatedOrderID);
        
    }

    public async Task<List<LiveOrderDTO>> GetStrategyLiveOrders()
    {
        return await GetLiveOrdersAsync(GetStrategyProfileKey());
    }

    public async Task<ScoreCardDTO> GetTraderScoreCard()
    {
        return await Task.FromResult(ModelTraderScoreCardDTO);

        //ModelTraderScoreCardDTO =  await GetScoreCardAsync(trader_key);
        //return ModelTraderScoreCardDTO;
    }

    public async Task<int> ProcessOrderForStrategy(NewOrderDTO order)
    {
        int oid = await ProcessOrderAsync(order);

        switch (order.OrderType)
        {
            case OMS.SharedKernel.Common.OrderType.OPEN:
                Console.WriteLine($"{GREEN}{oid} {CurrentStrategyAccount.strategy_name} ProcessOrderForStrategy: {order.UserID} {order.OrderAction} {order.OrderType}");
                Console.ResetColor();
                break;
            case OMS.SharedKernel.Common.OrderType.CLOSE:
                Console.WriteLine($"{RED}{oid} {CurrentStrategyAccount.strategy_name} ProcessOrderForStrategy: {order.UserID} {order.OrderAction} {order.OrderType}");
                Console.ResetColor();
                break;

            default:
                Console.WriteLine(oid + " " + CurrentStrategyAccount.strategy_name + " ProcessOrderForStrategy: " + order.UserID + " " + order.OrderAction + " " + order.OrderType);
                break;                            
        }

        return await Task.FromResult(oid);
    }

}
