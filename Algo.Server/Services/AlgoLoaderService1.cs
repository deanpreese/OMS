using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMS.Grains.Interfaces;
using System.Threading.Channels;
using System.Text.Json;

using Algo.Trader.Models;
using System.Text;
using OMS.Services.Trading;
using OMS.Core.Common;
using Algo.Trader.Trader;
using Algo.Trader.Utility;

namespace Algo.Server.Services;

public class AlgoLoaderService1 : BackgroundService
{  
    private string algo_to_load = "NG1.json";
    private readonly AlgoOrderQueue _algoOrderQueue;
    private readonly ILogger<AlgoLoaderService1> _logger;
    private AlgoData _algoData; 
    ChannelReader<LiveOrder> _reader;
    private readonly IClusterClient _client;

    public AlgoLoaderService1(ILogger<AlgoLoaderService1> logger, 
            AlgoOrderQueue algoOrderQueue,
            IClusterClient client)
    {
        _algoOrderQueue = algoOrderQueue;
        _logger = logger;
        _client = client;

        _algoData = new AlgoData();
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
            SimpleOpenClose algo = new SimpleOpenClose(_client, _algoData);

            await foreach (var newOrderInfo in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    NewOrder n_order = await algo.GenerateAlgoOrder(newOrderInfo);

                    IOrderGrain orderGrain = _client.GetGrain<IOrderGrain>(_algoData.algo_grain());     
                    await orderGrain.ProcessOrder(n_order);
                    
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
