using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;

using OMS.Core.DTO;

using OMS.Core.Models;
using System.Security.Cryptography;
using OMS.Infrastructure.Services.Data;

namespace OMS.Infrastructure.Services.Data;

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
