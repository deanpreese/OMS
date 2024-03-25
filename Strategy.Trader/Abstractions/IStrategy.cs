using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Strategy.Trader.Abstractions;

public interface IStrategy
{
   Task<NewOrderDTO> OnNewOrder(ModelOrderLogDTO _orig_live_order);     
}



