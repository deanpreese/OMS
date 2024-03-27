using OMS.SharedKernel;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;
using Strategy.Trader.Abstractions;

namespace Strategy.Server.Providers;

public class InMemoryStrategyConnection : StrategyApiClient, IStrategyConnection
{
    StrategyAccount _strategyAccount;
    ScoreCardDTO _scoreCardDTO;
    LiveOrderDTO _liveOrderDTO;
    ClosedTradeDTO  _closedTradeDTO;
    List<LiveOrderDTO> _liveOrders;

    public InMemoryStrategyConnection(string baseURL)
    {
        base.BaseUrl = baseURL;
    }

    private string GetProfileKey()
    {
        return _strategyAccount.strategy_traderId + "_" + _strategyAccount.group;
    }



    public async Task Initialize(StrategyAccount strategyAccount)
    {
        _strategyAccount = strategyAccount;

        NewTraderDTO n_strategy = new NewTraderDTO
        {
            UserID = 0,
            DisplayName = strategyAccount.strategy_name,
            GroupID = strategyAccount.group,
            UserPwd = "abc",
            FirstName = "Algo",
            LastName = strategyAccount.strategy_name,
            Email = "abc@abc"
        };

        int t_v = 0;
        await Task.Run(async () =>
        {
            t_v = await VerifyAndAddByDisplayNameAsync(n_strategy);
            n_strategy.UserID = t_v;
            await AddScoreCardForTraderAsync(n_strategy);
            _strategyAccount.strategy_traderId = t_v;
        });
    }
        
    
    public async Task OnTraderModelData(ModelOrderLogDTO modelOrderLogDTO)
    {
        _scoreCardDTO =modelOrderLogDTO.ScoreCardDeserialized;
        _liveOrderDTO = modelOrderLogDTO.LiveOrderDeserialized;
        _closedTradeDTO = modelOrderLogDTO.ClosedTradeDeserialized;

        _liveOrders =  await GetLiveOrdersAsync(GetProfileKey());   

        await Task.CompletedTask;

    }
    
    public async Task<StrategyAccount> GetStrategyAccount()
    {
        return await Task.FromResult(_strategyAccount) ;
    }

    public async Task<ClosedTradeDTO> RefreshLastClosedTraderTradeByOpenPlatformID(int traderPlatformId)
    {
        _closedTradeDTO =  await GetLastClosedTradeByOpenPlatformIDAsync(GetProfileKey(), traderPlatformId);
        return _closedTradeDTO;
    }

    public Task<ClosedTradeDTO> GetLastClosedTraderTradeByOpenPlatformID(int traderPlatformId)
    {
        if(_closedTradeDTO.OpenPlatformOrderID == traderPlatformId)
        {
            return Task.FromResult(_closedTradeDTO);
        }else
        {
            return  RefreshLastClosedTraderTradeByOpenPlatformID(traderPlatformId);
        }
        
    }


    public Task<List<LiveOrderDTO>> GetStrategyLiveOrders()
    {
        return Task.FromResult(_liveOrders);
    }

    
    public async Task<List<LiveOrderDTO>> RefreshStrategyLiveOrders()
    {
        _liveOrders =  await GetLiveOrdersAsync(GetProfileKey());
        return _liveOrders;
    }

    public Task<ScoreCardDTO> GetTraderScoreCard()
    {
        return Task.FromResult(_scoreCardDTO);
    }

    public async Task<ScoreCardDTO> RefreshTraderScoreCard()
    {
        _scoreCardDTO =  await GetScoreCardAsync(GetProfileKey());
        return _scoreCardDTO;
    }   

    public async Task<int> ProcessOrderForStrategy(NewOrderDTO order)
    {
        Console.WriteLine("Strategy Server: ProcessOrderForStrategy: " + order.UserID + " " + order.OrderAction + " " + order.OrderType);
        return await Task.FromResult(0);
    }

}
