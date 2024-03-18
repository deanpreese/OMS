using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

using OMS.Core.Models;

using Orleans.Streams;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Strategy.Trader;

public class OrderBackgroundService : BackgroundService
{
    private readonly IClusterClient _client;
    private readonly ILogger<OrderBackgroundService> _logger;
    private readonly StrategyOrderQueue _strategyOrderQueue;
    private IAsyncStream<LiveOrder> openOrderStreamProvider;


    public OrderBackgroundService(IClusterClient client, 
        StrategyOrderQueue strategyOrderQueue, 
        ILogger<OrderBackgroundService> logger)
    {
        _client = client;
        _logger = logger;
        _strategyOrderQueue = strategyOrderQueue;
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
                await _strategyOrderQueue.WriteAsync(newLiveOrder);
            });
    }
}
