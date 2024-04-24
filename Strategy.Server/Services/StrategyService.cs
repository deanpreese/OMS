
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

    ScreenColorBase _scb = new ScreenColorBase();

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
        loadedStrategy = await StrategyLoader.LoadStrategy(strategy_to_load);

        await base.StartAsync(cancellationToken);
    }

    
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       await Task.Run(async () =>
       {
            await foreach (ModelOrderLogDTO modelOrderLogDataDTO in _reader.ReadAllAsync(stoppingToken))
            {
                var dto = await EvaluateStrategy(modelOrderLogDataDTO);

                Console.WriteLine($"{_scb.CYAN} ORDER: {dto.UserID} {dto.OrderAction}");
                Console.ResetColor();
                
            }
        });
        await Task.CompletedTask;
    }

    public async Task<NewOrderDTO> EvaluateStrategy(ModelOrderLogDTO modelOrderLogDataDTO)
    {
        return await loadedStrategy.OnTraderModelData(modelOrderLogDataDTO); 
    }


}
