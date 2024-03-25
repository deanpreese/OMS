using OMS.Application.Models;
using OMS.Application.Common;

namespace OMS.Application.Interfaces;

public interface ILiveOrderRepository
{
    Task AddLiveOrderAsync(LiveOrder o);
    Task DeleteOrderAsyncByOrderManagerID( int orderManagerID );
    Task<List<LiveOrder>> GetOrdersByTrader(int UserID, string instrument);
    Task<List<LiveOrder>> GetOrdersByTraderAsync( int UserID, int GroupNumber );
    Task<List<LiveOrder>> GetLiveOrders(  int GroupNumber );


}
