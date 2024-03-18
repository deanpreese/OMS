using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;
using OMS.Core.Models;
using System.Security.Cryptography;
using OMS.Core.DTO;


namespace OMS.Infrastructure.Services.Data;

public interface IDataService
{
    Task<List<LiveOrder>> GetLiveOrdersByTrader(int user, int group);
    Task<List<ClosedTrade>> GetClosedOrdersByTrader(int user, int group);
    Task<List<UserInfoDTO>> GetActiveTraders(int group);
    Task<List<LiveOrder>> GetAllLiveOrders(int group);
}
