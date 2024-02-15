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
using OMS.Grains.Interfaces;

//using System.Timers;


namespace OMS.Services.Queue;

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

        var _timer = new System.Threading.Timer(
            callback: async state => { 
                await RunSync(stoppingToken);
                 },
            state: null,
            dueTime: TimeSpan.Zero,  // Start immediately
            period: TimeSpan.FromSeconds(10));
        
        await Task.CompletedTask;
    }

    private async Task RunSync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Syncing Closed Orders ... " );

        await foreach (var userInfo in _closedChannelService.ReadAllAsync(stoppingToken))
        {
            // Add a HashSet to keep track of distinct user IDs
            HashSet<int> distinctUserIds = new HashSet<int>();

            // Check if the user ID is already processed
            if (distinctUserIds.Contains(userInfo.UserID))
            {
                Console.WriteLine("User already processed: " + userInfo.UserID);
                continue; // Skip processing if already processed
            }

            // Add the user ID to the HashSet
            distinctUserIds.Add(userInfo.UserID);

            // Process the closed order for the distinct user ID
            try
            {
                await ProcessClosedOrderAsync(userInfo, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        await Task.CompletedTask;
    }



    private async Task ProcessClosedOrderAsync(UserInfo userInfo, CancellationToken cancellationToken)
    {
            using (var scope = _scopeFactory.CreateScope())
            {

                try 
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
                    UnitOfWork unitOfWork = new UnitOfWork(scopedContext);
                    ILogger<AnalyticsService> logger = scope.ServiceProvider.GetRequiredService<ILogger<AnalyticsService>>();
                    AnalyticsService _analytics_service = new AnalyticsService(unitOfWork, logger);   

                    await _analytics_service.UpdateScoreCard(userInfo);

                    var client = _clusterClient.ServiceProvider.GetRequiredService<IClusterClient>();
                    var orderStreamProvider = client.GetStreamProvider(PlatformConstants.OrderStreamProvider)
                                .GetStream<OrderInfo>(PlatformConstants.MemoryStreamNamespace, "/orders");

                    OrderInfo c_o =  new OrderInfo
                    {
                        UserID = userInfo.UserID,
                        GroupNumber = userInfo.GroupNumber,
                        InfoType = OrderInfoType.CLOSED,
                    };

                    await orderStreamProvider.OnNextAsync(c_o);

                    string g_k = userInfo.UserID + "_" + userInfo.GroupNumber;
                    ITraderGrain trader =  _grainFactory.GetGrain<ITraderGrain>(g_k);
                    await trader.Update(g_k);


                    AnsiConsole.MarkupLine("Order Processed by Stats For user " + userInfo.UserID  );

                }catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
                    
            }
        await Task.CompletedTask; 
    }
}
