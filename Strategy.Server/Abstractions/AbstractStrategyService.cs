
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


namespace Strategy.Server.Abstractions;

public abstract class AbstractStrategyService : BackgroundService
{
    private ChannelReader<ModelOrderLogDTO> _reader;
    private IncomingOrderQueue _strategyOrderQueue;
    ILogger _logger;
    BufferBlock<ModelOrderLogDTO> flowBuffer;

    public IStrategyConnection _strategyConnection;
    public StrategyAccount _strategyAccount;
    public IStrategy loadedStrategy ;

    public AbstractStrategyService(ILogger logger, IncomingOrderQueue strategyOrderQueue) 
    {
        _strategyOrderQueue = strategyOrderQueue;
        _reader = _strategyOrderQueue.Subscribe();
        _logger = logger;

        loadedStrategy = new NStrategy();
        flowBuffer = new BufferBlock<ModelOrderLogDTO>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });
        Task.Run(async () => await Distribute());
    }
    
    /*
    public override  async Task StartAsync(CancellationToken cancellationToken)
    {
        string strategy_to_load = "NG3.json";
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load);

        _strategyConnection = await configLoader.GetStrategyConnection();
        _strategyAccount = await _strategyConnection.GetStrategyAccount();

        loadedStrategy = new BaseFadeStrategy(_strategyConnection );

        await base.StartAsync(cancellationToken);
    }
    */

    
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
            ModelOrderLogDTO newLiveOrder = flowBuffer.Receive();
            await loadedStrategy.OnTraderModelData(newLiveOrder);
        }
    }

}


