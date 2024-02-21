using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMS.Grains.Interfaces;
using System.Threading.Channels;
using System.Text.Json;

using Algo.Algorithms.Models;
using Algo.Algorithms.Services;
using System.Text;
using OMS.Services.Trading;

namespace Algo.Algorithms.Services;

public class AlgoLoaderService3 : BackgroundService
{  
    private string algo_to_load = "Full.json";

    private readonly AlgoOrderQueue _algoOrderQueue;
    private readonly ILogger<AlgoLoaderService3> _logger;
    private readonly IGrainFactory _grainFactory;
    private readonly ChannelReader<LiveOrder> _reader;
    private AlgoData _algoData; 
    private readonly IClusterClient _client;
    
    public AlgoLoaderService3(ILogger<AlgoLoaderService3> logger, 
            AlgoOrderQueue algoOrderQueue,
            IGrainFactory grainFactory, IClusterClient client)
    {
        _algoOrderQueue = algoOrderQueue;
        _logger = logger;
        _grainFactory = grainFactory;
        _client = client;
    
        _algoData = new AlgoData();
        _reader = _algoOrderQueue.Subscribe();
       
    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
       AlgoConfig configLoader = new AlgoConfig(algo_to_load, _grainFactory, _client);
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
                    ITraderGrain trader =  _grainFactory.GetGrain<ITraderGrain>(g_k);
                    await trader.Update(g_k);
                    UserProfile u = await trader.GetProfileAsync();
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
