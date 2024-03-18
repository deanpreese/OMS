using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OMS.Core.DTO;


using System.Threading.Channels;
using System.Text.Json;

using System.Text;
using OMS.Core.Common;
using Strategy.Trader.Models;
using Strategy.Trader.Utility;
using Strategy.Trader.Strategy;
using OMS.Core.Interfaces;
using Strategy.Trader.Abstractions;
using Strategy.Trader;
using System.Threading.Tasks.Dataflow;

namespace Strategy.Trader.Services;

public class CounterService : BackgroundService
{  
    private ChannelReader<LiveOrder> _reader;
    private StrategyOrderQueue _strategyOrderQueue;
    ILogger<CounterService> _logger;
    private IStrategy loadedStrategy ;
    IClusterClient _clusterClient;
    StrategyAccount _strategyAccount;
    
    BufferBlock<LiveOrder> flowBuffer;
    
    public CounterService(ILogger<CounterService> logger, StrategyOrderQueue strategyOrderQueue, IClusterClient client) 
    {
        _strategyOrderQueue = strategyOrderQueue;
        _reader = _strategyOrderQueue.Subscribe();
        _logger = logger;
        _clusterClient = client;

        _strategyAccount = new StrategyAccount();
        loadedStrategy = new NStrategy();

        flowBuffer = new BufferBlock<LiveOrder>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });
        Task.Run(async () => await Distribute());
    }
    
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        string strategy_to_load = "NG0.json";
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load, _clusterClient);
        _strategyAccount= configLoader.GetStrategyData().Result;

        loadedStrategy = new BaseCounterStrategy(_clusterClient, _strategyAccount);

        return base.StartAsync(cancellationToken);
    }


    
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       await Task.Run(async () =>
       {
            await foreach (LiveOrder newLiveOrder in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await flowBuffer.SendAsync(newLiveOrder); 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        });
        await Task.CompletedTask;
    }



    private async Task Distribute()    
    {
         while (await flowBuffer.OutputAvailableAsync()) 
        {
            int delay = flowBuffer.Count > 100 ? 25 : flowBuffer.Count;
            await Task.Delay(25);       
            LiveOrder newLiveOrder = flowBuffer.Receive();
            NewOrderDTO n_order = await loadedStrategy.OnNewOrder(newLiveOrder);
        }
    }


}
