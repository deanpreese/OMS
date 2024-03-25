using System;
using System.Threading.Tasks;
using OMS.Application.Interfaces;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

using OMS.Application.Models;
using System.Security.Cryptography;
using OMS.Infrastructure.Interfaces;

namespace OMS.Infrastructure.Services;

public class DataService : IDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public DataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }   

    public Task<List<UserInfoDTO>> GetActiveTraders(int groupNumber)
    {
        throw new NotImplementedException();
    }

    
    public async Task<List<LiveOrder>> GetAllLiveOrders(int groupNumber)
    {
        List<LiveOrder> liveOrders = new List<LiveOrder>();    
        liveOrders = await _unitOfWork.LiveOrderRepository.GetLiveOrders(groupNumber);
        return liveOrders;
    }

    public Task<List<ClosedTrade>> GetClosedOrdersByTrader(int user, int groupNumber)
    {
        throw new NotImplementedException();
    }

    public Task<List<LiveOrder>> GetLiveOrdersByTrader(int user, int groupNumber)
    {
        throw new NotImplementedException();
    }
}
