
using Strategy.Trader.Abstractions;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;

namespace Strategy.Trader;

public class NStrategy :  IStrategy
{

    public NStrategy()
    {
    }

    public Task<int> EvaluateFilters(ScoreCardDTO scoreCard)
    {
        throw new NotImplementedException();
    }

    public  Task<NewOrderDTO> OnNewData(LiveOrderDTO traderLiveOrderDTO)
    {
        throw new NotImplementedException();
    }

    public Task OnTraderModelData(ModelOrderLogDTO modelOrderLogDTO)
    {
        throw new NotImplementedException();
    }
}
