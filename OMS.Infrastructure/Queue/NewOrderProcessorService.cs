using OMS.Application.Models;
using OMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using OMS.Application.Common;
using Orleans.Streams;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using OMS.Infrastructure.Services.Common;

using OMS.Infrastructure.Data;
using OMS.Infrastructure.Services;

using OMS.Infrastructure.Interfaces;
using OMS.Application;

namespace OMS.Infrastructure.Queue;

public class NewOrderProcessorService : BackgroundService
{
    private readonly object _lock = new object();

    private readonly NewOrderChannelService _orderChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NewOrderProcessorService> _logger;
    private IPlatformOrderIDGen _platformOrderIDGen;

    //private readonly IClusterClient _clusterClient;
    
    public NewOrderProcessorService(ILogger<NewOrderProcessorService> logger,
            NewOrderChannelService newOrderChannelService,
            IServiceScopeFactory scopeFactory,
                IServiceProvider serviceProvider,
                IPlatformOrderIDGen platformOrderIDGen
              )
    {
        _orderChannelService = newOrderChannelService;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _platformOrderIDGen = platformOrderIDGen;
        //_clusterClient = clusterClient;
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

    private async Task ProcessLiveOrderAsync(NewOrderDTO newOrderDTO, CancellationToken cancellationToken)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var scopedContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
            UnitOfWork unitOfWork = new UnitOfWork(scopedContext);
            ILogger<TradingService> logger = scope.ServiceProvider.GetRequiredService<ILogger<TradingService>>();

            ClosedOrderChannelService closedOrderChannel = scope.ServiceProvider.GetRequiredService<ClosedOrderChannelService>();
            TradingService _trader_service = new TradingService(unitOfWork, logger, _platformOrderIDGen);

            ILogger<DataService> aLogger = scope.ServiceProvider.GetRequiredService<ILogger<DataService>>();
            DataService _dataService = new DataService(unitOfWork);
                        
            await Task.Run(async () =>
            {
                LiveOrder liveOrder = await _trader_service.ProcessNewOrderAsync(newOrderDTO);
                LogDataDTO logData = new LogDataDTO { liveOrder = liveOrder, newOrderDTO = newOrderDTO };

                ILogger<AnalyticsService> aLogger = scope.ServiceProvider.GetRequiredService<ILogger<AnalyticsService>>();
                AnalyticsService _analytics_service = new AnalyticsService(unitOfWork, aLogger);

                await closedOrderChannel.WriteAsync(logData);
                
                int om_id = liveOrder.OrderManagerID;
                if (om_id != 0)
                {
                    if (newOrderDTO.GroupID < 50)
                    {
                        ClosedTradeDTO closedTrade = new ClosedTradeDTO();

                        if (liveOrder.OrderType  == OrderType.CLOSE )  
                        {
                            await _analytics_service.UpdateTraderScoreCard(liveOrder);
                            Console.WriteLine("New Analytics For Trader " + liveOrder.UserID  ); 

                            closedTrade =await _dataService.GetLastClosedTradeForTrader(liveOrder.UserID, liveOrder.GroupID);
                        }

                        await _analytics_service.LogModelOrderData(liveOrder, newOrderDTO,closedTrade);  

                        /*
                        var client = _clusterClient.ServiceProvider.GetRequiredService<IClusterClient>();
                        var orderStreamProvider = client.GetStreamProvider(PlatformConstants.OrderStreamProvider)
                                    .GetStream<LiveOrderDTO>(PlatformConstants.MemoryStreamNamespace, "/new-orders");

                        LiveOrderDTO liveDTO = await DTOMapping.MapOrderLiveToLiveDTO(liveOrder); 

                        await orderStreamProvider.OnNextAsync(liveDTO);
                        */
                    }
                }
                else
                {
                    Console.WriteLine("Error Processing " + newOrderDTO.UserID + "  " + liveOrder.PlatformOrderID);
                }

            });

        }
        await Task.CompletedTask;
    }
}
