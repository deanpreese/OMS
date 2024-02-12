using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;
using OMS.Core.Common;
using OMS.Data.Repositories;
using OMS.Core.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using OMS.Services.Queue;
using OMS.Services.Common;

namespace OMS.Services.Trading;

public class TradingService : ITradingService
{
    private readonly IUnitOfWork _unitOfWork;
    private ILogger<TradingService> _logger;
    private readonly ClosedOrderChannelService _closedOrderChannelService;
    private IPlatformOrderIDGen _platformOrderIDGen;
    
    public TradingService(IUnitOfWork unitOfWork, ILogger<TradingService> logger, 
        ClosedOrderChannelService closedOrderChannelService, IPlatformOrderIDGen platformOrderIDGen)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _closedOrderChannelService = closedOrderChannelService;
        _platformOrderIDGen = platformOrderIDGen;

    }

    public async Task<int> AddTraderAsync(NewTrader newTrader)
    {
        int traderID = 0;

        try
        {
            _logger.LogInformation("Adding new trader...");
            // Use TraderRepository to add a new trader
            traderID = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            // Commit transaction
            _unitOfWork.Commit();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        _logger.LogInformation($"Trader added  {traderID}  ");
        return traderID;
    }

    public async Task<int> AuthenticateTraderAsync(UserInfo newTrader)
    {
        int auth_code = 0;    

        if (newTrader.Password != null)
        {
            auth_code = await _unitOfWork.TraderRepository.AuthenticateTraderAsync(newTrader.UserID, newTrader.Password, newTrader.GroupNumber);    
            _unitOfWork.Commit();
        }    

        return auth_code;
    }


    // Example method to handle a new order
    public async Task<LiveOrder> ProcessNewOrderAsync(NewOrder newOrder)
    {

        LiveOrder liveOrder = MapOrder(newOrder);
        await _unitOfWork.UnderCoverRepository.AddToOrderFlowAsync(liveOrder);
        _unitOfWork.Commit();

        await ProcessNewOrder(liveOrder, _unitOfWork.OrderRepository);
        _unitOfWork.Commit();

        return liveOrder;
    }


    private LiveOrder MapOrder(NewOrder newOrder)
    {
        int om_id = 0;
        using (var generator = RandomNumberGenerator.Create())
        {
            var salt = new byte[4];
            generator.GetBytes(salt);
            om_id = BitConverter.ToInt32(salt, 0);
        }

        LiveOrder liveOrder = OrderMapping.MapOrder(newOrder);
        liveOrder.OrderManagerID = om_id;
        return liveOrder;
    }


    public async Task<double> ProcessNewOrder(LiveOrder newOrder, IOrderRepository orderRepository)
    {
        double r_val = 100.0;
        try
        {
            List<LiveOrder> orders = await orderRepository.GetOrdersByTrader(newOrder.UserID, newOrder.Instrument);

            if (orders.Any())
            {
                ProcessExistingOrders(orders, newOrder, orderRepository);
            }
            else
            {
                r_val = ProcessNewPosition(newOrder, orderRepository);
            }
        }
        catch (Exception fail)
        {
            Console.WriteLine(fail.Message);
            r_val = 0;
        }

        return await Task.FromResult(r_val);
    }


    private async void ProcessExistingOrders(List<LiveOrder> orders, LiveOrder newOrder, IOrderRepository orderRepository)
    {
        LiveOrder existingOrder = orders.First();


        if ((existingOrder.OrderAction == OrderAction.Buy && newOrder.OrderAction == OrderAction.Buy) ||
                (existingOrder.OrderAction == OrderAction.Sell && newOrder.OrderAction == OrderAction.Sell))
        {
            double rtn_val =  ProcessNewPosition(newOrder, orderRepository );
            //Console.WriteLine("Adding to existing position "  +  existingOrder.OrderManagerID + " " + rtn_val);
        }
        else 
        {
            // means that the - existingOrder - is the order to close
            // For right now we assume that the single order quantity = newOrder quantity
            //if (orders.Count == 1)
            {
                //Console.WriteLine("Closing an existing position "  +  existingOrder.OrderManagerID);
                await CloseOrder(existingOrder, newOrder, orderRepository);
            }
            // Need to figure out how to close out these orders
        }
    }


    public double ProcessNewPosition(LiveOrder order, IOrderRepository orderRepository)
    {
        //Console.WriteLine("OM -- Processing New Position "  +  order.OrderManagerID);
        orderRepository.AddLiveOrderAsync(order);

        return order.OrderManagerID;
    }


    // ******************************************************************************************* /
    public async Task<double> CloseOrder(LiveOrder orderToClose, LiveOrder orderToStore, IOrderRepository orderRepository)
    {
        ClosedTrade histOrder = OrderMapping.CloseOrder(orderToClose, orderToStore);       

        histOrder.MAE = orderToClose.MAE; 
        histOrder.MFE = orderToClose.MFE;

        double PNL = 0;
        double netChange = 0;

        PNL = InstrumentUtility.CalcPNL(orderToStore.Instrument, orderToStore.OrderAction, orderToClose.OrderPX, orderToStore.OrderPX, orderToStore.Quantity);
        netChange = InstrumentUtility.CalcNetDollars(orderToStore.Instrument, PNL);

        histOrder.PNL = PNL;
        histOrder.NetChange = netChange;

        UserInfo userInfo  = new UserInfo();
        userInfo.UserID = orderToClose.UserID;
        userInfo.GroupNumber = orderToClose.UserGroup;
        userInfo.Password = "abc";

        try 
        {
            await orderRepository.AddClosedOrder(histOrder);
            await orderRepository.DeleteOrderAsyncByOrderManagerID(orderToClose.OrderManagerID);
            await _closedOrderChannelService.WriteAsync(userInfo);

        }catch (Exception fail)
        {
            Console.WriteLine(fail.Message);
        }
        return orderToClose.OrderManagerID;
    }


}
