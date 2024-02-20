using Orleans;
using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using OMS.Grains.Interfaces;
using Orleans.Runtime;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orleans.Core;
using OMS.Core.Interfaces;

using OMS.Data.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using OMS.Services.Queue;

namespace OMS.Grains.Impl;

public class OrderGrain : Grain, IOrderGrain
{
    private readonly NewOrderChannelService _newOrderChannelService;
    IUnitOfWork _unitOfWork;

    public OrderGrain(IUnitOfWork unitOfWork, NewOrderChannelService newOrderChannelService)
    {
        _newOrderChannelService = newOrderChannelService;
        _unitOfWork = unitOfWork;
    }

    public Task<int> ProcessOrder(NewOrder order)
    {
        throw new NotImplementedException();
    }
}
