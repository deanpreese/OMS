
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;
using System.Threading.Channels;

using Strategy.Trader.Services;
using Strategy.Trader.Models;
using Strategy.Trader.Utility;
using Strategy.Trader.Strategy;
using Strategy.Trader.Abstractions;
using OMS.SharedKernel.DTO;


namespace Strategy.Trader.StrategyServices;



public class BasicService : BackgroundService
{
private ChannelReader<LiveOrderDTO> _reader;
    private IncomingOrderQueue _incomingOrderQueue;
    ILogger<BasicService> _logger;
    private IStrategy loadedStrategy ;

    IClusterClient _clusterClient;
    StrategyAccount _strategyAccount;

    BufferBlock<LiveOrderDTO> flowBuffer;

    public BasicService(ILogger<BasicService> logger, IncomingOrderQueue incomingOrderQueue, IClusterClient client) 
    {
        _incomingOrderQueue = incomingOrderQueue;
        _reader = _incomingOrderQueue.Subscribe();
        _logger = logger;
        _clusterClient = client;
        _strategyAccount = new StrategyAccount();
        loadedStrategy = new NStrategy();

        flowBuffer = new BufferBlock<LiveOrderDTO>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });
        Task.Run(async () => await Distribute());
        
    }

    public override  Task StartAsync(CancellationToken cancellationToken)
    {
        string strategy_to_load = "NG1.json";
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load, _clusterClient);
        _strategyAccount= configLoader.GetStrategyData().Result;
        loadedStrategy = new OpenCloseStrategy(_clusterClient, _strategyAccount);
        return base.StartAsync(cancellationToken);
    }


   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       await Task.Run(async () =>
       {
            await foreach (LiveOrderDTO newLiveOrder in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await flowBuffer.SendAsync(newLiveOrder); 
                    //await loadedStrategy.OnNewOrder(newLiveOrder);
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
              
            if(flowBuffer.Count > 10)
                Console.WriteLine("****** " + _strategyAccount.strategy_name + " HIGH Buffer Count: " + flowBuffer.Count);
 
            LiveOrderDTO newLiveOrder = flowBuffer.Receive();
            await loadedStrategy.OnNewOrder(newLiveOrder);
        }
    }


}
