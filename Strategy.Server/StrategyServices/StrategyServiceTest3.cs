using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OMS.SharedKernel.DTO;
using Strategy.Server.Services;
using Strategy.SharedKernel;

//using Strategy.Trader;
//using Strategy.Trader.Abstractions;

namespace Strategy.Server;

public class StrategyServiceTest3: BackgroundService
{  
    private ChannelReader<ModelOrderLogDTO> _reader;
    private IncomingOrderQueue _strategyOrderQueue;
    ILogger<StrategyServiceTest3> _logger;
    //private IStrategy loadedStrategy ;
    StrategyAccount _strategyAccount;
    BufferBlock<ModelOrderLogDTO> flowBuffer;

    int _itemCount ;

    
    public StrategyServiceTest3(ILogger<StrategyServiceTest3> logger, IncomingOrderQueue strategyOrderQueue) 
    {
        _strategyOrderQueue = strategyOrderQueue;
        _reader = _strategyOrderQueue.Subscribe();
        _logger = logger;
        _itemCount = 0;
        _strategyAccount = new StrategyAccount();
        //loadedStrategy = new NStrategy();

        flowBuffer = new BufferBlock<ModelOrderLogDTO>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });
        Task.Run(async () => await Distribute());
    }
    
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _strategyAccount = new StrategyAccount();
        _strategyAccount.strategy_name = "NG2";
        _strategyAccount.strategy_traderId = new Random().Next(5000, 7000);
        _strategyAccount.group = 1;
        _strategyAccount.strategy_class = "Strategy.Trader.Strategy.BaseFollowStrategy";

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

            if(flowBuffer.Count > 1)
                Console.WriteLine("****** " + _strategyAccount.strategy_name + " HIGH Buffer Count: " + flowBuffer.Count);

            _itemCount++;

            ModelOrderLogDTO newLiveOrder = flowBuffer.Receive();
            //await loadedStrategy.OnNewOrder(newLiveOrder);

            Console.WriteLine("3 >>> "  +  _strategyAccount.strategy_traderId + " " + _itemCount );
            
        }
    }


}
