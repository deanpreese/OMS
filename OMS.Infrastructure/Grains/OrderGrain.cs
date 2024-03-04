using OMS.Core.Models;

using OMS.Core.Interfaces;
using OMS.Infrastructure.Services.Queue;


namespace OMS.Infrastructure.Grains;
public class OrderGrain : Grain, IOrderGrain
{
    private readonly NewOrderChannelService _newOrderChannelService;
    IUnitOfWork _unitOfWork;

    public OrderGrain(IUnitOfWork unitOfWork, NewOrderChannelService newOrderChannelService)
    {
        _newOrderChannelService = newOrderChannelService;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> ProcessOrder(NewOrder order)
    {
        await _newOrderChannelService.WriteAsync(order);
        return 0;
    }
}
