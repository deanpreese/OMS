using OMS.Core.Models;

using System.Text;
using OMS.Core.WebAPIClient;
using Strategy.Trader.Models;

namespace Strategy.Trader.Utility;


public class AlgoConfig
{
    StrategyAccount _algoData;
    string _filePath;
    IClusterClient _clusterClient;

    public AlgoConfig(string filePath, IClusterClient clusterClient)
    {
        _algoData = new StrategyAccount();
        _filePath = filePath;
        _clusterClient = clusterClient;
    }

    public async Task<StrategyAccount> GetAlgoData()
    {
        await LoadConfig(_filePath);
        return await VerifyAlgoTraders();
    }

    private async Task<StrategyAccount> LoadConfig(string filePath)
    {
        string jsonString = await File.ReadAllTextAsync(filePath);
        _algoData = await System.Text.Json.JsonSerializer.DeserializeAsync<StrategyAccount>(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))) ??
            throw new ArgumentNullException($"File {filePath} is empty.");

        Console.WriteLine($"{_algoData.startegy_name} Started" + jsonString);
        return _algoData;
    }

    private async Task<StrategyAccount> VerifyAlgoTraders()
    {

        NewTrader n_trader = new NewTrader
        {
            UserID = 0,
            DisplayName = _algoData.startegy_name,
            GroupID = _algoData.group,
            UserPwd = "abc",
            FirstName = "Algo",
            LastName = _algoData.startegy_name,
            Email = "abc@abc"
        };

        int t_v = 0;
        await Task.Run(async () =>
        {
            t_v = await OMSClient.VerifyModelTrader(n_trader);
            Thread.Sleep(2000);
        });



        _algoData.strategy_traderId = t_v;

        Console.WriteLine("Trader " + t_v);

        return _algoData;
    }



}
