using OMS.SharedKernel.DTO;
using Strategy.Runner.Provider;
using OMS.SharedKernel.Common;
using System.Text;
using Strategy.Trader.Abstractions;
using System.Reflection;


namespace Strategy.Runner.Utility;


public class StrategyConfig
{
    string _filePath;
    public StrategyAccount StrategyAccountData ;
    

    public async Task<IStrategy> LoadStrategy(string filePath)
    {
        _filePath = filePath;

        string jsonString = await File.ReadAllTextAsync(filePath);
        StrategyAccountData = await System.Text.Json.JsonSerializer.DeserializeAsync<StrategyAccount>(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))) ??
            throw new ArgumentNullException($"File {filePath} is empty.");

        IStrategyConnection _strategyConnection = new InMemoryStrategyConnection();
        await _strategyConnection.Initialize(StrategyAccountData);

        Console.WriteLine("  ");
        Console.WriteLine($"{StrategyAccountData.strategy_name} Loaded");
        Console.WriteLine( jsonString);


        Assembly assembly = Assembly.Load(StrategyAccountData.strategy_assembly);
        Type myType = assembly.GetType(StrategyAccountData.strategy_class);
        return  (IStrategy)Activator.CreateInstance(myType, _strategyConnection);
    }
}
