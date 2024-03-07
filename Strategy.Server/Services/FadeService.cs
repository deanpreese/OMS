using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading.Channels;
using OMS.Core.Common;
using Strategy.Trader.Strategy;
using Strategy.Trader.Utility;
using Strategy.Trader.Models;
using OMS.Core.Interfaces;

namespace Strategy.Server.Services;

public class FadeService : BackgroundService
{  
    private string algo_to_load = "NG3.json";
    private readonly StrategyOrderQueue _algoOrderQueue;
    private readonly ILogger<FadeService> _logger;
    private StrategyAccount _algoData; 
    ChannelReader<LiveOrder> _reader;
    private readonly IClusterClient _client;

    public FadeService(ILogger<FadeService> logger, 
            StrategyOrderQueue algoOrderQueue,
            IClusterClient client)
    {
        _algoOrderQueue = algoOrderQueue;
        _logger = logger;
        _client = client;

        _algoData = new StrategyAccount();
        _reader = _algoOrderQueue.Subscribe();

    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        StrategyConfig configLoader = new StrategyConfig(algo_to_load, _client);
        _algoData = configLoader.GetStrategyData().Result;
        await base.StartAsync(cancellationToken);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

       await Task.Run(async () =>
        {
            BaseFadeStrategy strategy = new BaseFadeStrategy(_client, _algoData);

            await foreach (var newOrderInfo in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    NewOrder n_order = await strategy.OnNewOrder(newOrderInfo);
                    if(n_order.OrderAction != OrderAction.NoAction)
                    {
                        IOrderGrain orderGrain = _client.GetGrain<IOrderGrain>(_algoData.strategy_grain_key());     
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
