using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.SharedKernel.Common;

public static class SharedMapping
{
    public async static Task<NewOrderDTO> MapLiveOrderDTOLiveToNewDTO(LiveOrderDTO liveOrder)
    {
        NewOrderDTO newOrder = new NewOrderDTO {
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