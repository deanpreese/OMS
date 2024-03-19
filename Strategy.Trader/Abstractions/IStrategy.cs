
using OMS.Core.Models;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Strategy.Trader.Abstractions;


public interface IStrategy
{
   Task<NewOrderDTO> OnNewOrder(LiveOrder _orig_live_order);     
}



