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


namespace Algo.Algorithms.Services;

public class OrderBackgroundService : BackgroundService
{
    private readonly IClusterClient _client;
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<OrderBackgroundService> _logger;
    private readonly AlgoOrderQueue _algoOrderQueue;

    private IAsyncStream<LiveOrder>? openOrderStreamProvider;


    public OrderBackgroundService(IClusterClient client, 
        AlgoOrderQueue algoOrderQueue, 
        ILogger<OrderBackgroundService> logger, 
        IGrainFactory grainFactory)
    {
        _client = client;
        _logger = logger;
        _algoOrderQueue = algoOrderQueue;
        _grainFactory = grainFactory;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        openOrderStreamProvider = _client.GetStreamProvider(PlatformConstants.OrderStreamProvider)
                    .GetStream<LiveOrder>(PlatformConstants.MemoryStreamNamespace, "/new-orders");

        return base.StartAsync(cancellationToken);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await openOrderStreamProvider.SubscribeAsync(
            async (newLiveOrder, token) =>
            {

                try
                {
                    //Console.WriteLine("Order Info: " + orderInfo.UserID + "  " + orderInfo.GroupNumber + "  " + orderInfo.InfoType);
                    string g_k = newLiveOrder.UserID + "_" + newLiveOrder.UserGroup;
                    ITraderGrain trader =  _grainFactory.GetGrain<ITraderGrain>(g_k);
                    await trader.Update(g_k);
                    UserProfile u = await trader.GetProfileAsync();
                    Console.WriteLine("------ AlgoNG2 for : " + g_k + "  " + u.UserID + "  " +  "  " + newLiveOrder.OrderAction);
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }



                await _algoOrderQueue.WriteAsync(newLiveOrder);
            });
    }
}
