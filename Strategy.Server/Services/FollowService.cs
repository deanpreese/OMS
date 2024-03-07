using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Threading.Channels;
using System.Text.Json;

using System.Text;
using OMS.Core.Common;
using Strategy.Trader.Models;
using Strategy.Trader.Utility;
using Strategy.Trader.Strategy;
using OMS.Core.Interfaces;

namespace Strategy.Server.Services;

public class FollowService : BackgroundService
{  
    private string strategy_to_load = "NG2.json";
    private readonly StrategyOrderQueue _strategyOrderQueue;
    private readonly ILogger<FollowService> _logger;
    private StrategyAccount _strategyData; 
    ChannelReader<LiveOrder> _reader;
    private readonly IClusterClient _client;

    public FollowService(ILogger<FollowService> logger, 
            StrategyOrderQueue strategyOrderQueue,
            IClusterClient client)
    {
        _strategyOrderQueue = strategyOrderQueue;
        _logger = logger;
        _client = client;

        _strategyData = new StrategyAccount();
        _reader = _strategyOrderQueue.Subscribe();

    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load, _client);
        _strategyData = configLoader.GetStrategyData().Result;
        await base.StartAsync(cancellationToken);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       await Task.Run(async () =>
        {
            BaseFollowStrategy strategy = new BaseFollowStrategy(_client, _strategyData);

            await foreach (LiveOrder newLiveOrder in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    NewOrder n_order = await strategy.OnNewOrder(newLiveOrder);
                    if(n_order.OrderAction != OrderAction.NoAction)
                    {
                        IOrderGrain orderGrain = _client.GetGrain<IOrderGrain>(_strategyData.strategy_grain_key());     
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
