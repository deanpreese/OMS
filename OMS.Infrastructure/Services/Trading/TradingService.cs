using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;
using OMS.Core.Common;
using OMS.Core.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using OMS.Infrastructure.Services.Queue;
using OMS.Infrastructure.Services.Common;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;



namespace OMS.Infrastructure.Services.Trading;

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

    public async Task<LiveOrder> ProcessNewOrderAsync(NewOrderDTO newOrder)
    {
        LiveOrder newLiveOrder = await MapAndAddOrderManagerID(newOrder);
        await _unitOfWork.UnderCoverRepository.AddToOrderFlowAsync(newLiveOrder);
        await _unitOfWork.CommitAsync();

        ILiveOrderRepository liveOrderRepository = _unitOfWork.LiveOrderRepository;
        IClosedOrderRepository closedOrderRepository = _unitOfWork.ClosedOrderRepository;

        List<LiveOrder> orders = await liveOrderRepository.GetOrdersByTrader(newOrder.UserID, newOrder.Instrument);

        if (orders.Any())
        {
            LiveOrder existingOrder = orders.First();

            if ((existingOrder.OrderAction == OrderAction.Buy && newOrder.OrderAction == OrderAction.Buy) ||
                    (existingOrder.OrderAction == OrderAction.Sell && newOrder.OrderAction == OrderAction.Sell))
            {
                newLiveOrder.OrderType = OrderType.OPEN;
                await ProcessNewPosition(newLiveOrder, liveOrderRepository );
                await _unitOfWork.CommitAsync();
                
                
            }
            else 
            {
                // means that the - existingOrder - is the order to close
                // For right now we assume that the single order quantity = newOrder quantity
                //if (orders.Count == 1)
                {
                    //Console.WriteLine("Closing an existing position "  +  existingOrder.OrderManagerID);
                    newLiveOrder.OrderType = OrderType.CLOSE;
                    await CloseOrder(existingOrder, newLiveOrder, liveOrderRepository, closedOrderRepository);
                    await _unitOfWork.CommitAsync();

                    await _closedOrderChannelService.WriteAsync(newLiveOrder);
                }
                // Need to figure out how to close out these orders
            }
        }
        else
        {
            newLiveOrder.OrderType = OrderType.OPEN;
            await ProcessNewPosition(newLiveOrder, liveOrderRepository);
            await _unitOfWork.CommitAsync();
        }

        //_unitOfWork.Commit();
        return newLiveOrder;
    }





    public async Task ProcessNewPosition(LiveOrder order, ILiveOrderRepository liveOrderRepository)
    {
        //Console.WriteLine("OM -- Processing New Position "  +  order.OrderManagerID);
        await liveOrderRepository.AddLiveOrderAsync(order);
        await Task.CompletedTask;            
    }


    // ******************************************************************************************* /
    public async Task<double> CloseOrder(LiveOrder orderToClose, LiveOrder orderToStore, 
        ILiveOrderRepository liveOrderRepository, IClosedOrderRepository closedOrderRepository)
    {
        ClosedTrade histOrder = await DTOMapping.MapClosedOrder(orderToClose, orderToStore);       

        histOrder.MAE = orderToClose.MAE; 
        histOrder.MFE = orderToClose.MFE;

        double PNL = 0;
        double netChange = 0;

        PNL = InstrumentUtility.CalcPNL(orderToStore.Instrument, orderToStore.OrderAction, orderToClose.OrderPX, orderToStore.OrderPX, orderToStore.Quantity);
        netChange = InstrumentUtility.CalcNetDollars(orderToStore.Instrument, PNL);

        histOrder.PNL = PNL;
        histOrder.NetChange = netChange;


        try 
        {
            await closedOrderRepository.AddClosedOrder(histOrder);
            await liveOrderRepository.DeleteOrderAsyncByOrderManagerID(orderToClose.OrderManagerID);

        }catch (Exception fail)
        {
            Console.WriteLine(fail.Message);
        }
        return orderToClose.OrderManagerID;
    }



    private async Task<LiveOrder> MapAndAddOrderManagerID(NewOrderDTO newOrder)
    {
        newOrder.PlatformOrderID = _platformOrderIDGen.GetNextOrderID();

        int om_id = 0;
        using (var generator = RandomNumberGenerator.Create())
        {
            var salt = new byte[4];
            generator.GetBytes(salt);
            om_id = BitConverter.ToInt32(salt, 0);
        }

        LiveOrder liveOrder = await DTOMapping.MapOrderNewToLive(newOrder);
        liveOrder.OrderManagerID = om_id;

        return await Task.FromResult(liveOrder);
    }


}
