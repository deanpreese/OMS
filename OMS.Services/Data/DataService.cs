using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using OMS.Core.Models;
using System.Security.Cryptography;

namespace OMS.Services.Data;

public class DataService : IDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public DataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }   

    public Task<List<UserInfo>> GetActiveTraders(int groupNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ClosedTrade>> GetAllClosedTrades(int groupNumber)
    {
        List<ClosedTrade> closedTrades = new List<ClosedTrade>();    
        closedTrades = await _unitOfWork.OrderRepository.GetClosedTrades(groupNumber);
        return closedTrades;
    }

    public async Task<List<LiveOrder>> GetAllLiveOrders(int groupNumber)
    {
        List<LiveOrder> liveOrders = new List<LiveOrder>();    
        liveOrders = await _unitOfWork.OrderRepository.GetLiveOrders(groupNumber);
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
