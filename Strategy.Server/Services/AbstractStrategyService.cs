using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading.Channels;
using OMS.Core.Common;
using Strategy.Trader.Strategy;
using Strategy.Trader.Utility;
using Strategy.Trader.Models;
using OMS.Core.Interfaces;
using Strategy.Trader.Abstractions;
using Strategy.Trader;

namespace Strategy.Server.Services;

public class AbstractStrategyService : BackgroundService
{
    private ChannelReader<LiveOrder> _reader;
    
    private StrategyOrderQueue _strategyOrderQueue;
    private ILogger<AbstractStrategyService> _logger;

    public  IClusterClient clusterClient { get; set; }
    public StrategyAccount strategyAccount { get; set; }   

    public IStrategy loadedStrategy = new NStrategy(); 

    public AbstractStrategyService(ILogger<AbstractStrategyService> logger, 
            StrategyOrderQueue strategyOrderQueue,
            IClusterClient client)
    {
        _strategyOrderQueue = strategyOrderQueue;
        _logger = logger;
        clusterClient = client;

        strategyAccount = new StrategyAccount();
        _reader = _strategyOrderQueue.Subscribe();

    }

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       await Task.Run(async () =>
       {
            await foreach (LiveOrder newLiveOrder in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    Thread.Sleep(25);
                    NewOrder n_order = await loadedStrategy.OnNewOrder(newLiveOrder);
                    if(n_order.OrderAction != OrderAction.NoAction)
                    {
                        IOrderGrain orderGrain = clusterClient.GetGrain<IOrderGrain>(strategyAccount.strategy_grain_key());     
                        await orderGrain.ProcessOrder(n_order);
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        });
        await Task.CompletedTask;
    }
}
