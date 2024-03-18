using OMS.Core.Models;
using OMS.Core.Common;

namespace OMS.Core.Interfaces;

public interface ILiveOrderRepository
{
    Task AddLiveOrderAsync(LiveOrder o);
    Task DeleteOrderAsyncByOrderManagerID( int orderManagerID );
    Task<List<LiveOrder>> GetOrdersByTrader(int UserID, string instrument);
    Task<List<LiveOrder>> GetOrdersByTraderAsync( int UserID, int GroupNumber );
    Task<List<LiveOrder>> GetLiveOrders(  int GroupNumber );


}
