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

namespace Strategy.Trader.Services;

public class AbstractStrategyService : BackgroundService
{
    private ChannelReader<LiveOrder> _reader;
    private StrategyOrderQueue _strategyOrderQueue;
    ILogger<AbstractStrategyService> _logger;
    public IStrategy loadedStrategy ;

    public AbstractStrategyService(ILogger<AbstractStrategyService> logger, 
            StrategyOrderQueue strategyOrderQueue)
    {
        _strategyOrderQueue = strategyOrderQueue;
        _reader = _strategyOrderQueue.Subscribe();
        _logger = logger;
        loadedStrategy = new NStrategy(null, null);

    }

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       await Task.Run(async () =>
       {
            await foreach (LiveOrder newLiveOrder in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await Task.Delay(25);
                    NewOrder n_order = await loadedStrategy.OnNewOrder(newLiveOrder);
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
