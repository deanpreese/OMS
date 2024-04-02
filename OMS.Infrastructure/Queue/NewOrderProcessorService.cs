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
    private readonly ILogger<NewOrderProcessorService> _logger;
    private IPlatformOrderIDGen _platformOrderIDGen;
    
    public NewOrderProcessorService(ILogger<NewOrderProcessorService> logger,
            NewOrderChannelService newOrderChannelService,
            IServiceScopeFactory scopeFactory,
                IPlatformOrderIDGen platformOrderIDGen
              )
    {
        _orderChannelService = newOrderChannelService;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _platformOrderIDGen = platformOrderIDGen;
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
            UnitOfWork tradingUnitOfWork = new UnitOfWork(scopedContext);
            ILogger<TradingService> logger = scope.ServiceProvider.GetRequiredService<ILogger<TradingService>>();
            
            ClosedOrderChannelService closedOrderChannel = scope.ServiceProvider.GetRequiredService<ClosedOrderChannelService>();
            TradingService _trader_service = new TradingService(tradingUnitOfWork, logger, _platformOrderIDGen);

            ILogger<DataService> aLogger = scope.ServiceProvider.GetRequiredService<ILogger<DataService>>();
            UnitOfWork dataUnitOfWork = new UnitOfWork(scopedContext);
                        
            await Task.Run(async () =>
            {
                try
                {



                LiveOrder liveOrder = await _trader_service.ProcessNewOrderAsync(newOrderDTO);
                
                if (newOrderDTO.GroupID < 50)
                {
                    ILogger<AnalyticsService> aLogger = scope.ServiceProvider.GetRequiredService<ILogger<AnalyticsService>>();
                    UnitOfWork analyticsUnitOfWork = new UnitOfWork(scopedContext);
                    AnalyticsService _analytics_service = new AnalyticsService(analyticsUnitOfWork, aLogger);

                    ClosedTradeDTO closedTrade = new ClosedTradeDTO();
                    ScoreCard scoreCard = await _analytics_service.GetTraderScoreCard(liveOrder.UserID, liveOrder.GroupID);

                    if (liveOrder.OrderType  == OrderType.CLOSE )  
                    {
                        scoreCard = await _analytics_service.UpdateTraderScoreCard(liveOrder);

                        Console.WriteLine("New Analytics For Trader " + liveOrder.UserID  ); 

                        DataService _dataService = new DataService(dataUnitOfWork);
                        closedTrade = await _dataService.GetLastClosedTradeForTrader(liveOrder.UserID, liveOrder.GroupID);
                    }

                    await _analytics_service.LogModelOrderData(liveOrder, newOrderDTO, closedTrade, scoreCard );  

                }
                await closedOrderChannel.WriteAsync(new LogDataDTO { liveOrder = liveOrder, newOrderDTO = newOrderDTO });

                
                }catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
            });
        }
       
        await Task.CompletedTask;
    }
}
