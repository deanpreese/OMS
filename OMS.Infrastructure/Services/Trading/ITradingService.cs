using OMS.Core.Models;
using OMS.Core.DTO;


namespace OMS.Infrastructure.Services.Trading;

public interface ITradingService
{
    //Task<int> AddTraderAsync(NewTrader newTrader);
    //Task<int> AuthenticateTraderAsync(UserInfo newTrader);
    Task<LiveOrder> ProcessNewOrderAsync(NewOrderDTO newOrder);
    //Task<int> VerifyModelTrader(NewTrader user);
}
