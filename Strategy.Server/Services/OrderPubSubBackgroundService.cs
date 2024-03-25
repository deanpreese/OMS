using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Strategy.Server.Services;

public class OrderPubSubBackgroundService : BackgroundService
{
    private readonly ILogger<OrderPubSubBackgroundService> _logger;
    private readonly IncomingOrderQueue _strategyOrderQueue;

    public OrderPubSubBackgroundService(
        IncomingOrderQueue strategyOrderQueue, 
        ILogger<OrderPubSubBackgroundService> logger)
    {
        _logger = logger;
        _strategyOrderQueue = strategyOrderQueue;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        /*    
        openOrderStreamProvider = _client.GetStreamProvider(PlatformConstants.OrderStreamProvider)
                    .GetStream<LiveOrderDTO>(PlatformConstants.MemoryStreamNamespace, "/new-orders");
        */    
        return base.StartAsync(cancellationToken);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        /*
        await openOrderStreamProvider.SubscribeAsync(
            async (newLiveOrder, token) =>
            {
                await _strategyOrderQueue.WriteAsync(newLiveOrder);
            });
            */
    }


}
