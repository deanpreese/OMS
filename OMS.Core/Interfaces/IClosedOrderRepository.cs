using OMS.Core.Models;
using OMS.Core.Common;

namespace OMS.Core.Interfaces;

public interface IClosedOrderRepository
{
    
    Task AddClosedOrder(ClosedTrade o);
    Task<ClosedTrade> GetLastClosedTrade(int UserID, int GroupNumber);
    Task<ClosedTrade> GetLastClosedTradeByOpenPlatformID(int UserID, int GroupNumber, int platform_id);
    //Task<List<ClosedTrade>> GetClosedTrades(  int GroupNumber );
    Task<List<ClosedTrade>> Get_XXX_ClosedOrdersByTrader(int userID,  int GroupNumber ,  int numOrders);
    Task<List<ClosedTrade>> GetClosedOrdersByTraderAsync(int UserID, int GroupNumber);

}
