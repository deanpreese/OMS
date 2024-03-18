using OMS.Core.Interfaces;
using OMS.Core.Models;
using OMS.Core.DTO;


namespace Strategy.Trader.Abstractions;


public interface IStrategy
{
   Task<NewOrderDTO> OnNewOrder(LiveOrder _orig_live_order);     
}



