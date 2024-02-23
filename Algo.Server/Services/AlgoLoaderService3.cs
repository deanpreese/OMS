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
using Algo.Trader;
using Algo.Trader.Utility;

namespace Algo.Server.Services;

public class AlgoLoaderService3 : BackgroundService
{  
    private string algo_to_load = "Full.json";

    private readonly AlgoOrderQueue _algoOrderQueue;
    private readonly ILogger<AlgoLoaderService3> _logger;
    private readonly ChannelReader<LiveOrder> _reader;
    private AlgoData _algoData; 
    private readonly IClusterClient _client;
    
    public AlgoLoaderService3(ILogger<AlgoLoaderService3> logger, 
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
       AlgoConfig configLoader = new AlgoConfig(algo_to_load,  _client);
       _algoData = configLoader.GetAlgoData().Result;        
       await base.StartAsync(cancellationToken);

    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

       await Task.Run(async () =>
        {
            await foreach (var orderInfo in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    //Console.WriteLine("Order Info: " + orderInfo.UserID + "  " + orderInfo.GroupNumber + "  " + orderInfo.InfoType);
                    string g_k = orderInfo.UserID + "_" + orderInfo.UserGroup;
                    ITraderGrain trader =  _client.GetGrain<ITraderGrain>(g_k);
                    await trader.Update(g_k);
                    UserProfile u = await trader.GetProfileAsync(g_k);
                    Console.WriteLine(" <Full> Processing order for : " + g_k + "  " + u.UserID + "  " +  "  " + orderInfo.OrderAction);
                    
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
