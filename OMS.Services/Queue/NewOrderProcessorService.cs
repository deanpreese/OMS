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
using OMS.Grains.Interfaces;


using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;

namespace OMS.Services.Queue;

public class NewOrderProcessorService : BackgroundService
{
    private readonly object _lock = new object();

    private readonly NewOrderChannelService _orderChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NewOrderProcessorService> _logger;
    private IPlatformOrderIDGen _platformOrderIDGen;

    private readonly IClusterClient _clusterClient;
    private readonly IGrainFactory _grainFactory;

    public NewOrderProcessorService( ILogger<NewOrderProcessorService> logger, 
            NewOrderChannelService newOrderChannelService,
            IServiceScopeFactory scopeFactory,
                IServiceProvider serviceProvider,
                IPlatformOrderIDGen platformOrderIDGen,
                IClusterClient clusterClient,
                IGrainFactory grainFactory
              )
    {
        _orderChannelService = newOrderChannelService;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _platformOrderIDGen = platformOrderIDGen;
        _clusterClient = clusterClient;
        _grainFactory = grainFactory;
       
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var liveOrder in _orderChannelService.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessLiveOrderAsync(liveOrder, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    private async Task ProcessLiveOrderAsync(NewOrder newOrder, CancellationToken cancellationToken)
    {
            using (var scope = _scopeFactory.CreateScope())
            {
                var scopedContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
                UnitOfWork unitOfWork = new UnitOfWork(scopedContext);
                ILogger<TradingService> logger = scope.ServiceProvider.GetRequiredService<ILogger<TradingService>>();

                ClosedOrderChannelService closedOrderChannel = scope.ServiceProvider.GetRequiredService<ClosedOrderChannelService>();
                TradingService _trader_service = new TradingService(unitOfWork, logger, closedOrderChannel, _platformOrderIDGen);   
                Task<LiveOrder> live = _trader_service.ProcessNewOrderAsync(newOrder);

                int om_id = live.Result.OrderManagerID;

                if (om_id != 0)
                {
                    var client = _clusterClient.ServiceProvider.GetRequiredService<IClusterClient>();
                    var orderStreamProvider = client.GetStreamProvider(PlatformConstants.OrderStreamProvider)
                                .GetStream<OrderInfo>(PlatformConstants.MemoryStreamNamespace, "/orders");

                    OrderInfo n_o =  new OrderInfo
                    {
                        UserID = newOrder.UserID,
                        GroupNumber = newOrder.UserGroup,
                        InfoType = OrderInfoType.OPEN,
                    };

                    await orderStreamProvider.OnNextAsync(n_o);

                    string g_k = newOrder.UserID + "_" + newOrder.UserGroup;
                    ITraderGrain trader =  _grainFactory.GetGrain<ITraderGrain>(g_k);
                    await trader.Update(g_k);


                }
                else
                {
                    AnsiConsole.MarkupLine("Error Processing " + newOrder.UserID + "  " +  live.Result.PlatformOrderID);
                }
                    
            }
        await Task.CompletedTask; 
    }
}
