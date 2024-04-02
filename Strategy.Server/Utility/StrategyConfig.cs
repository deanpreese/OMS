using OMS.SharedKernel.DTO;
using Strategy.Server.Providers;
using Strategy.SharedKernel;
using System.Text;


namespace Strategy.Server.Utility;


public class StrategyConfig
{
    string _filePath;

    public StrategyAccount StrategyAccountData ;

    public StrategyConfig(string filePath)
    {
        _filePath = filePath;
    }

    private async Task<StrategyAccount> LoadConfig(string filePath)
    {
        string jsonString = await File.ReadAllTextAsync(filePath);
        StrategyAccountData = await System.Text.Json.JsonSerializer.DeserializeAsync<StrategyAccount>(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))) ??
            throw new ArgumentNullException($"File {filePath} is empty.");

        Console.WriteLine("  ");
        Console.WriteLine($"{StrategyAccountData.strategy_name} Loaded");
        Console.WriteLine( jsonString);
        return StrategyAccountData;
    }


    public async Task<IStrategyConnection> GetStrategyConnection()
    {
        StrategyAccount account = await LoadConfig(_filePath);
        IStrategyConnection _strategyConnection = new InMemoryStrategyConnection("http://10.0.0.147:8786");
        await _strategyConnection.Initialize(account);

        Console.WriteLine("StrategyConnection Account: " + _strategyConnection.CurrentStrategyAccount.strategy_traderId);

        return _strategyConnection;
    }


}
