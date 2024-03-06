using OMS.Core.Models;


namespace Strategy.Trader.Abstractions;


public interface IStrategy
{
   public Task<NewOrder> OnNewOrder(LiveOrder order);
}
