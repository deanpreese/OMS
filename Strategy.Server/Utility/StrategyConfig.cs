using OMS.SharedKernel.DTO;
using System.Text;
using Strategy.Server.Models;

namespace Strategy.Server.Utility;


public class StrategyConfig
{
    StrategyAccount _strategyData;
    string _filePath;
    IClusterClient _clusterClient;

    public StrategyConfig(string filePath, IClusterClient clusterClient)
    {
        _strategyData = new StrategyAccount();
        _filePath = filePath;
        _clusterClient = clusterClient;
    }

    public async Task<StrategyAccount> GetStrategyData()
    {
        await LoadConfig(_filePath);
        return await VerifyStrategyTraders();
    }

    private async Task<StrategyAccount> LoadConfig(string filePath)
    {
        string jsonString = await File.ReadAllTextAsync(filePath);
        _strategyData = await System.Text.Json.JsonSerializer.DeserializeAsync<StrategyAccount>(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))) ??
            throw new ArgumentNullException($"File {filePath} is empty.");

        Console.WriteLine($"{_strategyData.strategy_name} Started" + jsonString);
        return _strategyData;
    }

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
            /*
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
            */           
            
        });

        _strategyData.strategy_traderId = t_v;
        Console.WriteLine("Trader " + t_v);
        return _strategyData;
    }



}
