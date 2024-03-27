using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Strategy.Trader.Abstractions;

public interface IStrategy
{
   Task OnTraderModelData(ModelOrderLogDTO modelOrderLogDTO);
   Task<NewOrderDTO> OnNewData(LiveOrderDTO traderLiveOrderDTO);   
   Task<int> EvaluateFilters(ScoreCardDTO scoreCard);  
  
}



