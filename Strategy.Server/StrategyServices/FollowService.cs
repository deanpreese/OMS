
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;
using System.Threading.Channels;
using System.Text.Json;
using System.Text;

using Strategy.Server.Services;
using Strategy.Server.Utility;

//using Strategy.Trader.Strategy;
//using Strategy.Trader.Abstractions;
//using Strategy.Trader;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;
using Strategy.Trader.Abstractions;


namespace Strategy.Server.StrategyServices;

public class FollowService : BackgroundService
{  
    private ChannelReader<ModelOrderLogDTO> _reader;
    private IncomingOrderQueue _strategyOrderQueue;
    ILogger<FollowService> _logger;
    private IStrategy loadedStrategy ;

    IStrategyConnection _strategyConnection;
    StrategyAccount _strategyAccount;    
    BufferBlock<ModelOrderLogDTO> flowBuffer;
    
    public FollowService(ILogger<FollowService> logger, IncomingOrderQueue strategyOrderQueue, IClusterClient client) 
    {
        _strategyOrderQueue = strategyOrderQueue;
        _reader = _strategyOrderQueue.Subscribe();
        _logger = logger;

        _strategyAccount = new StrategyAccount();
        //loadedStrategy = new NStrategy();

        flowBuffer = new BufferBlock<ModelOrderLogDTO>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });
        Task.Run(async () => await Distribute());
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        string strategy_to_load = "NG2.json";
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load);

        _strategyConnection = configLoader.GetStrategyConnection().Result;
        _strategyAccount = await _strategyConnection.GetStrategyAccount();

        //loadedStrategy = new BaseFollowStrategy(_clusterClient, _strategyAccount);

        await base.StartAsync(cancellationToken);
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


            ModelOrderLogDTO newLiveOrder = flowBuffer.Receive();
            //await loadedStrategy.OnNewOrder(newLiveOrder);
        }
    }


}
