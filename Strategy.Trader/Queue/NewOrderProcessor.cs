using OMS.Core.Models;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Strategy.Trader;

public class NewOrderProcessor : BackgroundService
{
    private readonly NewOrderQueue _orderChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NewOrderProcessor> _logger;
    
    public NewOrderProcessor( ILogger<NewOrderProcessor> logger, 
            NewOrderQueue newOrderChannelService,
            IServiceScopeFactory scopeFactory,
                IServiceProvider serviceProvider 
              )
    {
        _orderChannelService = newOrderChannelService;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
       
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
                LiveOrder orderToMatch = await OrderMapping.MapOrderNewToLive(orderFromList);
                LiveOrder orderToStore = await OrderMapping.MapOrderNewToLive(newOrder);
                ClosedTrade histOrder = await OrderMapping.MapClosedOrder(orderToMatch, orderToStore);     

                await _orderChannelService.AddToClosedOrdersList(histOrder);
                await _orderChannelService.RemoveFromNewOrdersList(orderFromList.PlatformOrderID);

        }else
        {
            
            await _orderChannelService.AddToNewOrdersList(newOrder);
        }



        
        await Task.CompletedTask; 
    }

    private async Task DisplayOrders()
    {
        await Task.Run(async () =>
        {
            Console.WriteLine(" ");
            List<NewOrder> openOrders = await _orderChannelService.GetNewOrdersList();
            Console.WriteLine( DateTime.UtcNow  + " Open Orders " +  openOrders.Count);
            
            foreach (NewOrder order in openOrders)
            {
                if (order.OrderAction > 0 )
                {
                    Console.WriteLine( DateTime.UtcNow  + " New Order " + order.PlatformOrderID + "   " + order.UserName + " " + order.Instrument + " " + order.OrderAction + " " + order.OrderPX );
                }else{
                    Console.WriteLine(DateTime.UtcNow +  " New Order " + order.PlatformOrderID + "   " + order.UserName + " " + order.Instrument + " " + order.OrderAction + " " + order.OrderPX );
                }
               
            }
            
            Console.WriteLine(" ");    
            Console.WriteLine( DateTime.UtcNow  + " Closed Orders ") ;
            List<ClosedTrade> closedOrders = await _orderChannelService.GetClosedOrdersList();

            

            if(closedOrders.Count > 5)
            {
                Console.WriteLine(DateTime.UtcNow  + " Closed Orders Count " +  closedOrders.Count);
            }else
            {
                foreach (ClosedTrade order in closedOrders)
                {
                    Console.WriteLine($"UserID: {order.UserID} " +
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

        });
        await Task.CompletedTask;

    }
   
}
