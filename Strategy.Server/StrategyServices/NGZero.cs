
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks.Dataflow;

using Strategy.Server.Utility;
using Strategy.Server.Services;


//using Strategy.Trader.Strategy;
//using Strategy.Trader.Abstractions;
//using Strategy.Trader;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;
using Strategy.Trader.Abstractions;
using Strategy.Trader;
using Strategy.Trader.Strategy;
using System.Diagnostics.Metrics;
using Strategy.Server.Abstractions;
using System.Reflection;

namespace Strategy.Server.StrategyServices;

public class NGZero :  BaseStrategyService
{
    public NGZero(IServiceScopeFactory serviceScopeFactory, ModelOrderMessageBus messageBus) : base(serviceScopeFactory, messageBus)
    {
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        string strategy_to_load = "NG0.json";
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load);
        _strategyConnection = await configLoader.GetStrategyConnection();
        Assembly assembly = Assembly.Load(configLoader.StrategyAccountData.strategy_assembly);
        Type myType = assembly.GetType(configLoader.StrategyAccountData.strategy_class);
        loadedStrategy = (IStrategy)Activator.CreateInstance(myType, _strategyConnection);
        await base.StartAsync(cancellationToken);
    }
}
