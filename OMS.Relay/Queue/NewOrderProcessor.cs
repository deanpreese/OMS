using OMS.Core.Models;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Data;
using Microsoft.AspNetCore.SignalR;
using OMS.Relay.SignalHub;

namespace OMS.Services.Queue;

public class NewOrderProcessor : BackgroundService
{
    private readonly NewOrderQueue _orderChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NewOrderProcessor> _logger;
    private IHubContext<SignalHub> _hubContext;
    
    public NewOrderProcessor( ILogger<NewOrderProcessor> logger, 
            NewOrderQueue newOrderChannelService,
            IServiceScopeFactory scopeFactory,
                IServiceProvider serviceProvider,
                IHubContext<SignalHub> hubContext 
              )
    {
        _orderChannelService = newOrderChannelService;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _hubContext = hubContext;
       
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {
            await foreach (var liveOrder in _orderChannelService.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await ProcessLiveOrderAsync(liveOrder, stoppingToken);
                    await DisplayOrders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        });
    }

    private async Task ProcessLiveOrderAsync(NewOrder newOrder, CancellationToken cancellationToken)
    {
        List<NewOrder> list = await _orderChannelService.GetOrdersList(newOrder);

        if (list.Count > 0)
        {
                NewOrder orderFromList = list.First();
                LiveOrder orderToMatch = OrderMapping.MapOrder(orderFromList);
                LiveOrder orderToStore = OrderMapping.MapOrder(newOrder);
                ClosedTrade histOrder = OrderMapping.CloseOrder(orderToMatch, orderToStore);     

                await _orderChannelService.AddToClosedOrdersList(histOrder);
                await _orderChannelService.RemoveFromNewOrdersList(orderFromList.PlatformOrderID);

        }else
        {
            
            await _orderChannelService.AddToNewOrdersList(newOrder);
        }

        AnsiConsole.MarkupLine(" ");    
        await Task.CompletedTask; 
    }

    private async Task DisplayOrders()
    {
        await Task.Run(async () =>
        {
            AnsiConsole.MarkupLine(" ");
            List<NewOrder> openOrders = await _orderChannelService.GetNewOrdersList();
            AnsiConsole.MarkupLine("[White]" + DateTime.UtcNow  + " Open Orders " +  openOrders.Count   + "[/]");
            
            foreach (NewOrder order in openOrders)
            {
                if (order.OrderAction > 0 )
                {
                    AnsiConsole.MarkupLine("[lightgreen]" +   DateTime.UtcNow  + " New Order " + order.PlatformOrderID + "   " + order.UserName + " " + order.Instrument + " " + order.OrderAction + " " + order.OrderPX + "[/]");
                }else{
                    AnsiConsole.MarkupLine("[indianred_1]" + DateTime.UtcNow +  " New Order " + order.PlatformOrderID + "   " + order.UserName + " " + order.Instrument + " " + order.OrderAction + " " + order.OrderPX + "[/]");
                }
               
            }
            
            AnsiConsole.MarkupLine(" ");    
            AnsiConsole.MarkupLine("[White]" + DateTime.UtcNow  + " Closed Orders " + "[/]");
            List<ClosedTrade> closedOrders = await _orderChannelService.GetClosedOrdersList();

            

            if(closedOrders.Count > 5)
            {
                AnsiConsole.MarkupLine("[grey82]" + DateTime.UtcNow  + " Closed Orders Count " +  closedOrders.Count  +"[/]");
            }else
            {
                foreach (ClosedTrade order in closedOrders)
                {
                    AnsiConsole.MarkupLine($"UserID: {order.UserID} " +
                                          $"UserGroup: {order.GroupID} " +
                                          $"Instrument: {order.Instrument} " +
                                          $"Quantity: {order.Quantity} " +
                                          $"OpenPlatformOrderID: {order.OpenPlatformOrderID} " +
                                          $"OpenOrderTime: {order.OpenOrderTime} " +
                                          $"OpenOrderPX: {order.OpenOrderPX} " +
                                          $"OpenOrderAction: {order.OpenOrderAction} " +
                                          $"ClosePlatformOrderID: {order.ClosePlatformOrderID} " +
                                          $"CloseOrderTime: {order.CloseOrderTime} " +
                                          $"CloseOrderPX: {order.CloseOrderPX} " +
                                          $"CloseOrderAction: {order.CloseOrderAction}");
                }
            }

            await _hubContext.Clients.AllExcept("ReceiveLiveOrders", "ReceiveClosedTrades").SendAsync("ReceiveLiveOrders", openOrders.Count);
            await _hubContext.Clients.AllExcept("ReceiveLiveOrders", "ReceiveClosedTrades").SendAsync("ReceiveClosedTrades", closedOrders.Count);
        });
        await Task.CompletedTask;

    }
   
}
