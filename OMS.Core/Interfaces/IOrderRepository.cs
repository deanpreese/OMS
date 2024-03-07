using OMS.Core.Models;
using OMS.Core.Common;

namespace OMS.Core.Interfaces;

public interface IOrderRepository
{
    Task AddLiveOrderAsync(LiveOrder o);
    Task<int> UpdateOrder(LiveOrder o);
    Task DeleteOrderAsyncByRTOrderID( int rtOrderID );
    Task DeleteOrderAsyncByOrderManagerID( int orderManagerID );



    Task<List<LiveOrder>> GetOrdersByTrader(int UserID, string instrument, OrderAction orderAction);
    Task<List<LiveOrder>> GetOrdersByTrader(int UserID, string instrument);
    Task<List<LiveOrder>> GetOrdersByInstrumentAsync( string instrument );
    Task<List<LiveOrder>> GetOrdersByGroupAsync( int group );
    Task<List<LiveOrder>> UpdateOrderPriceByExeutionIDAsync(int executionId, double price);
    Task<List<LiveOrder>> GetOrdersByPlatformIDAsync( int platformID );  
    Task<List<LiveOrder>> GetOrdersByExecutionIDAsync( int executionID ); 
    Task<List<LiveOrder>> GetOrdersByRelatedOrderIDAsync( int relatedOrderID ); 
    Task<List<LiveOrder>> GetOrdersByTraderAsync( int UserID, int GroupNumber );
    Task<List<LiveOrder>> GetLiveOrders(  int GroupNumber );


    Task AddClosedOrder(ClosedTrade o);
    Task<ClosedTrade> GetLastClosedTrade(int UserID, int GroupNumber);
    Task<ClosedTrade> GetLastClosedTradeByOpenPlatformID(int UserID, int GroupNumber, int platform_id);
    Task<List<ClosedTrade>> GetClosedTrades(  int GroupNumber );
    Task<List<ClosedTrade>> Get_XXX_ClosedOrdersByTrader(int userID,  int GroupNumber ,  int numOrders);
    Task<List<ClosedTrade>> GetClosedOrdersByTraderAsync(int UserID, int GroupNumber);

}
