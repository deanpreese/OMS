using System;
using System.Threading.Tasks;
using OMS.Application.Interfaces;
using OMS.Application.Models;
using System.Security.Cryptography;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;



namespace OMS.Infrastructure.Interfaces;


public interface IDataService
{
    Task<List<LiveOrder>> GetLiveOrdersByTrader(int user, int group);
    Task<List<ClosedTrade>> GetClosedOrdersByTrader(int user, int group);
    Task<List<UserInfoDTO>> GetActiveTraders(int group);
    Task<List<LiveOrder>> GetAllLiveOrders(int group);
}
