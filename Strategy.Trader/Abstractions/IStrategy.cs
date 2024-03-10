using OMS.Core.Interfaces;
using OMS.Core.Models;


namespace Strategy.Trader.Abstractions;


public interface IStrategy
{
   Task<NewOrder> OnNewOrder(LiveOrder _orig_live_order);     
}



