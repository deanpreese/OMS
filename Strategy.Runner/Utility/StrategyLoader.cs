
using System.Reflection;
using OMS.SharedKernel.Common;
using Strategy.Trader.Abstractions;

namespace Strategy.Runner.Utility;

public static class StrategyLoader 
{
    public static async Task<IStrategy> LoadStrategy(string strategyFile)
    {
        StrategyConfig configLoader = new StrategyConfig(strategyFile);
        IStrategyConnection _strategyConnection = await configLoader.GetStrategyConnection();
        Assembly assembly = Assembly.Load(configLoader.StrategyAccountData.strategy_assembly);
        Type myType = assembly.GetType(configLoader.StrategyAccountData.strategy_class);
        return  (IStrategy)Activator.CreateInstance(myType, _strategyConnection);
        
    }
}

