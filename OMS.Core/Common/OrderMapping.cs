using System.Reflection.Metadata.Ecma335;
using OMS.Core.Models;

namespace OMS.Core.Common;

public class OrderMapping
{
    public static async Task<ClosedTrade> MapClosedOrder(LiveOrder orderToClose, LiveOrder orderToStore)
    {
        ClosedTrade histOrder = new ClosedTrade()
        {
            UserID = orderToClose.UserID,
            GroupID = orderToClose.GroupID,
            Instrument = orderToClose.Instrument,
            Quantity = orderToClose.Quantity,
            Leverage = orderToClose.Leverage,
            OppositeTrader = false,

            OpenPlatformOrderID = orderToClose.PlatformOrderID,
            OpenOrderMangerID = orderToClose.OrderManagerID,
            OpenAuthToken = orderToClose.AuthToken,
            OpenExecutionID = orderToClose.ExecutedOrderID,
            OpenRelatedOrderID = orderToClose.RelatedOrderID,
            OpenOrderTime = orderToClose.OrderTime,
            OpenOrderPX = orderToClose.OrderPX,
            OpenOrderType = orderToClose.OrderType,
            OpenOrderAction = orderToClose.OrderAction,

            ClosePlatformOrderID = orderToStore.PlatformOrderID,
            ClosedOrderMangerID = orderToStore.OrderManagerID,
            CloseAuthToken = orderToStore.AuthToken,
            CloseExecutionID = orderToStore.ExecutedOrderID,
            CloseRelatedOrderID = orderToStore.RelatedOrderID,
            CloseOrderTime = orderToStore.OrderTime,
            CloseOrderPX = orderToStore.OrderPX,
            CloseOrderType = orderToStore.OrderType,
            CloseOrderAction = orderToStore.OrderAction
        };

        return await Task.FromResult(histOrder);
    }


    public static async Task<LiveOrder> MapOrderNewToLive(NewOrder newOrder)
    {
        LiveOrder liveOrder = new LiveOrder();
        liveOrder.OrderPX = newOrder.OrderPX;  
        liveOrder.OrderTime = newOrder.OrderTime;
        liveOrder.OrderManagerID = 0 ;
        liveOrder.Instrument = newOrder.Instrument;
        liveOrder.OrderAction = newOrder.OrderAction;
        liveOrder.OrderType = newOrder.OrderType;
        liveOrder.PlatformOrderID = newOrder.PlatformOrderID;
        liveOrder.Quantity = newOrder.Quantity;
        liveOrder.UserID = newOrder.UserID;
        liveOrder.GroupID = newOrder.GroupID;
        liveOrder.Leverage = 1;
        liveOrder.Opposite = 0;
        liveOrder.RelatedOrderID = newOrder.RelatedOrderID;

        return await Task.FromResult(liveOrder);
    }



    public async static Task<NewOrder> MapOrderLiveToNew(LiveOrder liveOrder)
    {
        NewOrder newOrder = new NewOrder {
            OrderPX = liveOrder.OrderPX , 
            OrderTime = liveOrder.OrderTime,
            Instrument = liveOrder.Instrument,
            OrderAction = liveOrder.OrderAction,
            OrderType = liveOrder.OrderType,
            PlatformOrderID = 0,
            Quantity = liveOrder.Quantity,
            UserID = 0,
            GroupID = 0,
        };
        return await Task.FromResult(newOrder);

    }
}

   