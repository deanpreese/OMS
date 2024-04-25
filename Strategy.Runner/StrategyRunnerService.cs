using OMS.SharedKernel.DTO;
using Strategy.Runner.Utility;
using Strategy.SharedKernel;
using Strategy.Trader.Abstractions;

namespace Strategy.Runner.Services;

public class StrategyRunnerService
{
    public IStrategy loadedStrategy ;
    public IStrategyConnection _strategyConnection;

    public StrategyRunnerService()
    { 
     
    }

    public async Task LoadService()
    {
       string strategy_to_load = "Strategy.json";
       loadedStrategy = await StrategyLoader.LoadStrategy(strategy_to_load);
       
    }

    public async Task<NewOrderDTO> EvaluateStrategy(ModelOrderLogDTO modelOrderLogDataDTO)
    {
        return await loadedStrategy.OnTraderModelData(modelOrderLogDataDTO); 
    }

}
