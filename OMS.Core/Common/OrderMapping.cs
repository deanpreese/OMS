using OMS.Core.Models;

namespace OMS.Core.Common;

public class OrderMapping
{
    public static ClosedTrade CloseOrder(LiveOrder orderToClose, LiveOrder orderToStore)
    {
        ClosedTrade histOrder = new ClosedTrade()
        {
            UserID = orderToClose.UserID,
            GroupID = orderToClose.UserGroup,
            Instrument = orderToClose.Instrument,
            Quantity = orderToClose.Quantity,
            Leverage = orderToClose.Leverage,
            OppositeTrader = false,
            OpenPlatformOrderID = orderToClose.PlatformOrderID,
            OpenAuthToken = orderToClose.AuthToken,
            OpenExecutionID = orderToClose.ExecutedOrderID,
            OpenRelatedOrderID = orderToClose.RelatedOrderID,
            OpenOrderTime = orderToClose.OrderTime,
            OpenOrderPX = orderToClose.OrderPX,
            OpenOrderType = orderToClose.OrderType,
            OpenOrderAction = orderToClose.OrderAction,
            ClosePlatformOrderID = orderToStore.PlatformOrderID,
            CloseAuthToken = orderToStore.AuthToken,
            CloseExecutionID = orderToStore.ExecutedOrderID,
            CloseRelatedOrderID = orderToStore.RelatedOrderID,
            CloseOrderTime = orderToStore.OrderTime,
            CloseOrderPX = orderToStore.OrderPX,
            CloseOrderType = orderToStore.OrderType,
            CloseOrderAction = orderToStore.OrderAction
        };

        return histOrder;
    }


    public static LiveOrder MapOrder(NewOrder newOrder)
    {
        LiveOrder liveOrder = new LiveOrder();
        liveOrder.OrderPX = newOrder.OrderPX;  
        liveOrder.OrderTime = newOrder.OrderTime;
        liveOrder.OrderManagerID = newOrder.PlatformOrderID ;
        liveOrder.Instrument = newOrder.Instrument;
        liveOrder.OrderAction = newOrder.OrderAction;
        liveOrder.OrderType = newOrder.OrderType;
        liveOrder.PlatformOrderID = newOrder.PlatformOrderID;
        liveOrder.Quantity = newOrder.Quantity;
        liveOrder.UserID = newOrder.UserID;
        liveOrder.UserGroup = newOrder.UserGroup;
        liveOrder.Leverage = 1;
        liveOrder.Opposite = 0;
        return liveOrder;
    }


}

   