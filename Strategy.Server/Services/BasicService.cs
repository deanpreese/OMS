using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Threading.Channels;
using System.Text.Json;
using OMS.Core.Common;
using Strategy.Trader.Models;
using Strategy.Trader.Utility;
using Strategy.Trader.Strategy;
using OMS.Core.Interfaces;
using Strategy.Trader.Abstractions;
using Strategy.Trader;

namespace Strategy.Server.Services;

public class BasicService : BackgroundService
{  
    private string strategy_to_load = "NG1.json";
    private readonly StrategyOrderQueue _strategyOrderQueue;
    private readonly ILogger<BasicService> _logger;
    private StrategyAccount _strategyAccount; 
    ChannelReader<LiveOrder> _reader;
    private readonly IClusterClient _client;

    IStrategy _strategy = new NStrategy();

    public BasicService(ILogger<BasicService> logger, 
            StrategyOrderQueue strategyOrderQueue,
            IClusterClient client)
    {
        _strategyOrderQueue = strategyOrderQueue;
        _logger = logger;
        _client = client;

        _strategyAccount = new StrategyAccount();
        _reader = _strategyOrderQueue.Subscribe();

    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load, _client);
        _strategyAccount = configLoader.GetStrategyData().Result;
        await base.StartAsync(cancellationToken);
        _strategy = new OpenCloseStrategy(_client, _strategyAccount);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       await Task.Run(async () =>
        {
            await foreach (var newOrderInfo in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    NewOrder n_order = await _strategy.OnNewOrder(newOrderInfo);

                    if(n_order.OrderAction != OrderAction.NoAction)
                    {
                        IOrderGrain orderGrain = _client.GetGrain<IOrderGrain>(_strategyAccount.strategy_grain_key());     
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
