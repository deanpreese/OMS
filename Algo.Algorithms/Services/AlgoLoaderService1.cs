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

namespace Algo.Algorithms.Services;

public class AlgoLoaderService1 : BackgroundService
{  
    private string algo_to_load = "NG1.json";
    private readonly AlgoOrderQueue _algoOrderQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AlgoLoaderService1> _logger;
    private readonly IClusterClient _clusterClient;
    private readonly IGrainFactory _grainFactory;
    private AlgoData _algoData; 
    ChannelReader<OrderInfo> _reader;
    

    public AlgoLoaderService1(ILogger<AlgoLoaderService1> logger, 
            AlgoOrderQueue algoOrderQueue,
            IServiceScopeFactory scopeFactory,
            IServiceProvider serviceProvider,IClusterClient clusterClient, IGrainFactory grainFactory)
    {
        _algoOrderQueue = algoOrderQueue;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _clusterClient = clusterClient;
        _grainFactory = grainFactory;
        _algoData = new AlgoData();

        _reader = _algoOrderQueue.Subscribe();
        _algoData = AlgoConfigLoader.LoadConfig(algo_to_load).Result;
       
    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

       await Task.Run(async () =>
        {
            //await foreach (var orderInfo in _algoOrderQueue.ReadAllAsync(stoppingToken))
            await foreach (var orderInfo in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    //Console.WriteLine("Order Info: " + orderInfo.UserID + "  " + orderInfo.GroupNumber + "  " + orderInfo.InfoType);
                    string g_k = orderInfo.UserID + "_" + orderInfo.GroupNumber;
                    ITraderGrain trader =  _grainFactory.GetGrain<ITraderGrain>(g_k);
                    await trader.Update(g_k);
                    UserProfile u = await trader.GetProfileAsync();
                    Console.WriteLine(" ---> AlgoNG1 for : " + g_k + "  " + u.UserID + "  " +  "  " + orderInfo.InfoType);
                    
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
