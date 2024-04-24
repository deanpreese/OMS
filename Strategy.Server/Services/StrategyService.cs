
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;
using System.Threading.Channels;

using Strategy.Server.Services;
using Strategy.Server.Utility;

//using Strategy.Trader.Strategy;
//using Strategy.Trader.Abstractions;
//using Strategy.Trader;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;
using Strategy.Trader.Abstractions;
using Strategy.Trader;
using Strategy.Trader.Strategy;

using System.Reflection;

namespace Strategy.Server.StrategyServices;



public class StrategyService : BackgroundService
{

    public ChannelReader<ModelOrderLogDTO> _reader;
    public ModelOrderMessageBus _messageBus;
    public ILogger _logger;
    public IStrategy loadedStrategy ;
    public IStrategyConnection _strategyConnection;
    public IServiceScopeFactory _serviceScopeFactory;
    int orderCount;

     public StrategyService(IServiceScopeFactory serviceScopeFactory, ModelOrderMessageBus messageBus) 
    {
         _messageBus = messageBus;
        _reader = _messageBus.Subscribe();
        _serviceScopeFactory = serviceScopeFactory;
        loadedStrategy = new NStrategy();
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        string strategy_to_load = "Strategy.json";
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load);
        _strategyConnection = await configLoader.GetStrategyConnection();
        Assembly assembly = Assembly.Load(configLoader.StrategyAccountData.strategy_assembly);
        Type myType = assembly.GetType(configLoader.StrategyAccountData.strategy_class);
        loadedStrategy = (IStrategy)Activator.CreateInstance(myType, _strategyConnection);
        await base.StartAsync(cancellationToken);
    }

    
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       await Task.Run(async () =>
       {
            await foreach (ModelOrderLogDTO modelOrderLogDataDTO in _reader.ReadAllAsync(stoppingToken))
            {
                orderCount++;
                await loadedStrategy.OnTraderModelData(modelOrderLogDataDTO); 
            }
        });
        await Task.CompletedTask;
    }

}
