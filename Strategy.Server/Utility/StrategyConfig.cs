using OMS.SharedKernel.DTO;
using Strategy.Server.Providers;
using Strategy.SharedKernel;
using System.Text;


namespace Strategy.Server.Utility;


public class StrategyConfig
{
    string _filePath;

    public StrategyConfig(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<IStrategyConnection> GetStrategyConnection()
    {
        StrategyAccount account = await LoadConfig(_filePath);
        IStrategyConnection _strategyConnection = new BaseStrategyConnection();
        await _strategyConnection.Initialize(account);
        return _strategyConnection;
    }

    private async Task<StrategyAccount> LoadConfig(string filePath)
    {
        string jsonString = await File.ReadAllTextAsync(filePath);
        StrategyAccount _strategyData = await System.Text.Json.JsonSerializer.DeserializeAsync<StrategyAccount>(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))) ??
            throw new ArgumentNullException($"File {filePath} is empty.");

        Console.WriteLine($"{_strategyData.strategy_name} Started" + jsonString);
        return _strategyData;
    }


}
