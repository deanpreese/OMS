using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

using OMS.Core.Models;

using Orleans.Streams;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Strategy.Trader;

public class OrderPubSubBackgroundService : BackgroundService
{
    private readonly IClusterClient _client;
    private readonly ILogger<OrderPubSubBackgroundService> _logger;
    private readonly IncomingOrderQueue _strategyOrderQueue;
    private IAsyncStream<LiveOrder> openOrderStreamProvider;

    public OrderPubSubBackgroundService(IClusterClient client, 
        IncomingOrderQueue strategyOrderQueue, 
        ILogger<OrderPubSubBackgroundService> logger)
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
