using System;
using System.Threading.Tasks;
using OMS.Application.Interfaces;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

using OMS.Application.Models;
using System.Security.Cryptography;
using OMS.Infrastructure.Interfaces;
using OMS.Application.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMS.Infrastructure.Data;
using OMS.Application;

namespace OMS.Infrastructure.Services;

public class OrderManagerService 
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderManagerService> _logger;
    private IPlatformOrderIDGen _platformOrderIDGen;
    
    public OrderManagerService(ILogger<OrderManagerService> logger,
            IServiceScopeFactory scopeFactory,
                IPlatformOrderIDGen platformOrderIDGen
              )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _platformOrderIDGen = platformOrderIDGen;

    }

    public async Task<ModelOrderLog> ProcessNewTraderOrder(NewOrderDTO newOrderDTO)
    {
        ModelOrderLog modelOrderLog = new ModelOrderLog();

        using (var scope = _scopeFactory.CreateScope())
        {
            var scopedContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
            UnitOfWork tradingUnitOfWork = new UnitOfWork(scopedContext);
                        
            await Task.Run(async () =>
            {
                try
                {
                    TradingService _trader_service = new TradingService(tradingUnitOfWork, _platformOrderIDGen);
                    LiveOrder liveOrder = await _trader_service.ProcessNewOrderAsync(newOrderDTO);
                    
                    ILogger<AnalyticsService> aLogger = scope.ServiceProvider.GetRequiredService<ILogger<AnalyticsService>>();
                    UnitOfWork analyticsUnitOfWork = new UnitOfWork(scopedContext);
                    AnalyticsService _analytics_service = new AnalyticsService(analyticsUnitOfWork);

                    ClosedTradeDTO closedTrade = new ClosedTradeDTO();
                    ScoreCard scoreCard = await _analytics_service.GetTraderScoreCard(liveOrder.UserID, liveOrder.GroupID);

                    if (liveOrder.OrderType  == OrderType.CLOSE )  
                    {
                        scoreCard = await _analytics_service.UpdateTraderScoreCard(liveOrder);

                        Console.WriteLine("New Analytics For Trader " + liveOrder.UserID  + "  " + liveOrder.GroupID); 

                        UnitOfWork dataUnitOfWork = new UnitOfWork(scopedContext);
                        DataService _dataService = new DataService(dataUnitOfWork);
                        closedTrade = await _dataService.GetLastClosedTradeForTrader(liveOrder.UserID, liveOrder.GroupID);
                    }

                    if (newOrderDTO.GroupID < 1000)
                    {
                        modelOrderLog = await _analytics_service.LogModelOrderData(liveOrder, newOrderDTO, closedTrade, scoreCard );  
                    }

                
                }catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
            });
        }
       
        return modelOrderLog;
    }


}