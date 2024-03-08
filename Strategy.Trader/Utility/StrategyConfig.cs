using OMS.Core.Models;

using System.Text;
using OMS.Core.WebAPIClient;
using Strategy.Trader.Models;
using OMS.Core.Interfaces;

namespace Strategy.Trader.Utility;


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

        NewTrader n_trader = new NewTrader
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
            //await Task.Delay(2000);
            IAdminGrain adminGrain = _clusterClient.GetGrain<IAdminGrain>("A"+_strategyData.group);     
            t_v = await adminGrain.VerifyAndAddByDisplayName(n_trader);
           // await Task.Delay(2000);
        });

        _strategyData.strategy_traderId = t_v;
        Console.WriteLine("Trader " + t_v);
        return _strategyData;
    }



}
