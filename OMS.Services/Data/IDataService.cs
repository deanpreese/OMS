using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using OMS.Core.Models;
using System.Security.Cryptography;

namespace OMS.Services.Data;

public interface IDataService
{
    Task<List<LiveOrder>> GetLiveOrdersByTrader(int user, int group);
    Task<List<ClosedTrade>> GetClosedOrdersByTrader(int user, int group);
    Task<List<UserInfo>> GetActiveTraders(int group);
    Task<List<LiveOrder>> GetAllLiveOrders(int group);  
    Task<List<ClosedTrade>> GetAllClosedTrades(int group);              
}
