using OMS.Core.Models;

namespace OMS.Services.Trading;

public interface ITradingService
{
    //Task<int> AddTraderAsync(NewTrader newTrader);
    //Task<int> AuthenticateTraderAsync(UserInfo newTrader);
    Task<LiveOrder> ProcessNewOrderAsync(NewOrder newOrder);
    //Task<int> VerifyModelTrader(NewTrader user);
}
