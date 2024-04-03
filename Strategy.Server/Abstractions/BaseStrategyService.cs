
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;

using Strategy.Server.Services;
using Strategy.Server.Utility;

using Strategy.Trader.Strategy;
using Strategy.Trader.Abstractions;
using Strategy.Trader;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.Extensions.DependencyInjection;


namespace Strategy.Server.Abstractions;

public  class BaseStrategyService : BackgroundService
{
    public ChannelReader<ModelOrderLogDTO> _reader;
    public ModelOrderMessageBus _messageBus;
    public ILogger _logger;
    public IStrategy loadedStrategy ;
    public IStrategyConnection _strategyConnection;
    public IServiceScopeFactory _serviceScopeFactory;
    int orderCount;

    public BaseStrategyService(IServiceScopeFactory serviceScopeFactory, ModelOrderMessageBus messageBus)  
    {
        _messageBus = messageBus;
        _reader = _messageBus.Subscribe();
        _serviceScopeFactory = serviceScopeFactory;
        loadedStrategy = new NStrategy();
    }
   


   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       await Task.Run(async () =>
       {
            await foreach (ModelOrderLogDTO modelOrderLogDataDTO in _reader.ReadAllAsync(stoppingToken))
            {
                orderCount++;
                //Console.WriteLine(_strategyConnection.GetStrategyAccount().logid + "  " + counter);

                await loadedStrategy.OnTraderModelData(modelOrderLogDataDTO); 
                //await loadedStrategy.OnNewData(modelOrderLogDataDTO.LiveOrderDeserialized);
            }
        });
        await Task.CompletedTask;
    }


}


