
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks.Dataflow;

using Strategy.Server.Models;
using Strategy.Server.Utility;
using Strategy.Trader.Strategy;
using Strategy.Trader.Abstractions;
using Strategy.Server.Services;
using Strategy.Trader;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Strategy.Server.StrategyServices;


public class CounterService : BackgroundService
{  
    private ChannelReader<ModelOrderLogDTO> _reader;
    private IncomingOrderQueue _strategyOrderQueue;
    ILogger<CounterService> _logger;
    private IStrategy loadedStrategy ;
    IClusterClient _clusterClient;
    StrategyAccount _strategyAccount;
    
    BufferBlock<ModelOrderLogDTO> flowBuffer;
    
    public CounterService(ILogger<CounterService> logger, IncomingOrderQueue strategyOrderQueue, IClusterClient client) 
    {
        _strategyOrderQueue = strategyOrderQueue;
        _reader = _strategyOrderQueue.Subscribe();
        _logger = logger;
        _clusterClient = client;

        _strategyAccount = new StrategyAccount();
        loadedStrategy = new NStrategy();

        flowBuffer = new BufferBlock<ModelOrderLogDTO>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });
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
            await foreach (ModelOrderLogDTO newLiveOrder in _reader.ReadAllAsync(stoppingToken))
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
            //int delay = flowBuffer.Count > 100 ? 25 : flowBuffer.Count;
            //await Task.Delay(delay);         
            if(flowBuffer.Count > 10)
                Console.WriteLine("****** " + _strategyAccount.strategy_name + " HIGH Buffer Count: " + flowBuffer.Count);
                
            ModelOrderLogDTO newLiveOrder = flowBuffer.Receive();
            await loadedStrategy.OnNewOrder(newLiveOrder);
        }
    }


}
