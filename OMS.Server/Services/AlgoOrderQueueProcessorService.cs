using OMS.Core.Models;
using OMS.Services.Trading;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using OMS.Data;
using OMS.Services.Common;
using OMS.Core.Common;
using Orleans.Streams;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMS.Services.Data;
using OMS.Services.Queue;
using OMS.Grains.Interfaces;

//using System.Timers;


namespace OMS.Server.Services;


public class AlgoOrderQueueProcessorService : BackgroundService
{  
    private readonly AlgoOrderQueue _algoOrderQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AlgoOrderQueueProcessorService> _logger;
    private readonly IClusterClient _clusterClient;
    private readonly IGrainFactory _grainFactory;

    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30); // Setting the interval to 30 seconds


    public AlgoOrderQueueProcessorService(ILogger<AlgoOrderQueueProcessorService> logger, 
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
       
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

       await Task.Run(async () =>
        {
            await foreach (var orderInfo in _algoOrderQueue.ReadAllAsync(stoppingToken))
            {
                try
                {
                    //Console.WriteLine("Order Info: " + orderInfo.UserID + "  " + orderInfo.GroupNumber + "  " + orderInfo.InfoType);
                    string g_k = orderInfo.UserID + "_" + orderInfo.GroupNumber;
                    ITraderGrain trader =  _grainFactory.GetGrain<ITraderGrain>(g_k);
                    await trader.Update(g_k);
                    UserProfile u = await trader.GetProfileAsync();
                    Console.WriteLine("Processing order for : " + g_k + "  " + u.UserID + "  " +  "  " + orderInfo.InfoType);
                    
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
