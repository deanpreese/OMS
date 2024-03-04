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

namespace Strategy.Server.Services;

public class BasicService : BackgroundService
{  
    private string algo_to_load = "NG1.json";
    private readonly StrategyOrderQueue _algoOrderQueue;
    private readonly ILogger<BasicService> _logger;
    private StrategyAccount _algoData; 
    ChannelReader<LiveOrder> _reader;
    private readonly IClusterClient _client;

    public BasicService(ILogger<BasicService> logger, 
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
        AlgoConfig configLoader = new AlgoConfig(algo_to_load, _client);
        _algoData = configLoader.GetAlgoData().Result;
        await base.StartAsync(cancellationToken);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

       await Task.Run(async () =>
        {
            OpenCloseStrategy algo = new OpenCloseStrategy(_client, _algoData);

            await foreach (var newOrderInfo in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    NewOrder n_order = await algo.GenerateAlgoOrder(newOrderInfo);

                    if(n_order.OrderAction != OrderAction.NoAction)
                    {
                        IOrderGrain orderGrain = _client.GetGrain<IOrderGrain>(_algoData.strategy_grain());     
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
