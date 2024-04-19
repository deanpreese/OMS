using OMS.Application.Models;
using OMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using OMS.Application.Common;
using Orleans.Streams;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OMS.Infrastructure.Data;
using System.Security.Cryptography.X509Certificates;
using OMS.Application;
using OMS.Infrastructure.Services;


namespace OMS.Infrastructure.Queue;

public class ClosedOrderProcessorService : BackgroundService
{  
    private readonly ClosedOrderChannelService _closedChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ClosedOrderProcessorService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10); 
    Timer timer;
    List<LiveOrder> orders = new List<LiveOrder>();


    public ClosedOrderProcessorService(ILogger<ClosedOrderProcessorService> logger, 
            ClosedOrderChannelService closedOrderChannelService,
            IServiceScopeFactory scopeFactory,
            IServiceProvider serviceProvider
            )
    {
        _closedChannelService = closedOrderChannelService;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
        timer = new Timer(ProcessScorecardUpdatesAsync, null, _interval, _interval);
       
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await foreach (var logDataDTO in _closedChannelService.ReadAllAsync(stoppingToken))
        {
            try
            {   if (logDataDTO.liveOrder.GroupID >= 50)
                    orders.Add(logDataDTO.liveOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        await Task.CompletedTask;
    }




    private async void ProcessScorecardUpdatesAsync(object state)
    {
        if (orders.Count > 0)
        {
            Console.WriteLine("Updating Scorecards ... " );
            var userGroupPairs = orders.Select(o => (userID: o.UserID, groupID: o.GroupID)).Distinct().ToList();

            foreach ((int userID, int groupID) in userGroupPairs)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    try 
                    {
                        var scopedContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
                        UnitOfWork unitOfWork = new UnitOfWork(scopedContext);
                        AnalyticsService _analytics_service = new AnalyticsService(unitOfWork);   
                        await _analytics_service.UpdateTraderScoreCard(userID, groupID);
                        Console.WriteLine("Stats Updated For Strategy " + userID + " " + groupID);

                    }catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }

            orders.Clear();            
        }else
        {
            Console.WriteLine("No New Strategy Scorecard Updates  " );
        }

     await Task.CompletedTask; 
    }
}
