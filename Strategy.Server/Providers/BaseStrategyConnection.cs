using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;
using Strategy.Trader.Abstractions;

namespace Strategy.Server.Providers;

public class BaseStrategyConnection : IStrategyConnection
{

    public Task Initialize(StrategyAccount strategyAccount)
    {
        throw new NotImplementedException();

    /*
    private async Task<StrategyAccount> VerifyStrategyTraders()
    {
        NewTraderDTO n_trader = new NewTraderDTO
        {
            UserID = 0,
            DisplayName = _strategyData.strategy_name,
            GroupID = _strategyData.group,
            UserPwd = "abc",
            FirstName = "Algo",
            LastName = _strategyData.strategy_name,
            Email = "abc@abc"
        };

        int t_v = 0;
        await Task.Run(async () =>
        {
            IAdminGrain adminGrain = _clusterClient.GetGrain<IAdminGrain>("A"+_strategyData.group);     
            t_v = await adminGrain.AuthByDisplayName(n_trader);

            if(t_v == 0)
            {
                t_v = await adminGrain.AddNewTrader(n_trader);
                await Task.Delay(1500);
                
                if (t_v != 0)
                {
                    n_trader.UserID = t_v;
                    await adminGrain.AddScoreCardForTrader(n_trader);   
                }
            } 
                      
            
        });

        _strategyData.strategy_traderId = t_v;
        Console.WriteLine("Trader " + t_v);
        return _strategyData;
        
    }
    */

    }
    
    public Task Connect()
    {
        throw new NotImplementedException();
    }

    public Task<ClosedTradeDTO> GetLastClosedTraderTradeByOpenPlatformID(int traderPlatformId)
    {
        throw new NotImplementedException();
    }

    public Task<StrategyAccount> GetStrategyAccount()
    {
        throw new NotImplementedException();
    }

    public Task<List<LiveOrderDTO>> GetStrategyLiveOrders()
    {
        throw new NotImplementedException();
    }

    public Task<List<LiveOrderDTO>> GetTraderLiveOrders()
    {
        throw new NotImplementedException();
    }

    public Task<ScoreCardDTO> GetTraderScoreCard()
    {
        throw new NotImplementedException();
    }


    public Task<int> ProcessOrderForStrategy(NewOrderDTO order)
    {
        throw new NotImplementedException();
    }
}
