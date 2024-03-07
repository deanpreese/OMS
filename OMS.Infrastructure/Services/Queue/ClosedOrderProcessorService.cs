using OMS.Core.Models;
using OMS.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using OMS.Core.Common;
using Orleans.Streams;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMS.Infrastructure.Data.Repositories;
using OMS.Infrastructure.Services.Data;
using OMS.Infrastructure.Data;


namespace OMS.Infrastructure.Services.Queue;

public class ClosedOrderProcessorService : BackgroundService
{  
    private readonly ClosedOrderChannelService _closedChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ClosedOrderProcessorService> _logger;
    private readonly IClusterClient _clusterClient;
    private readonly IGrainFactory _grainFactory;

    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30); // Setting the interval to 30 seconds


    public ClosedOrderProcessorService(ILogger<ClosedOrderProcessorService> logger, 
            ClosedOrderChannelService closedOrderChannelService,
            IServiceScopeFactory scopeFactory,
            IServiceProvider serviceProvider,IClusterClient clusterClient, IGrainFactory grainFactory)
    {
        _closedChannelService = closedOrderChannelService;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _clusterClient = clusterClient;
        _grainFactory = grainFactory;
       
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Syncing Closed Orders ... " );

        await foreach (var liveOrder in _closedChannelService.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessClosedOrderAsync(liveOrder, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        await Task.CompletedTask;
    }



    private async Task ProcessClosedOrderAsync(LiveOrder liveOrder, CancellationToken cancellationToken)
    {
            using (var scope = _scopeFactory.CreateScope())
            {

                try 
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
                    UnitOfWork unitOfWork = new UnitOfWork(scopedContext);
                    ILogger<AnalyticsService> logger = scope.ServiceProvider.GetRequiredService<ILogger<AnalyticsService>>();
                    AnalyticsService _analytics_service = new AnalyticsService(unitOfWork, logger);   

                    await _analytics_service.UpdateScoreCard(liveOrder);

                    Console.WriteLine("Order Processed by Stats For user " + liveOrder.UserID  );

                }catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
                    
            }
        await Task.CompletedTask; 
    }
}
