using OMS.SharedKernel.DTO;
using Strategy.Runner.Utility;
using OMS.SharedKernel.Common;
using Strategy.Trader.Abstractions;

namespace Strategy.Runner.Services;

public class StrategyRunnerService
{
    public IStrategy loadedStrategy ;
    public IStrategyConnection _strategyConnection;

    List<IStrategy> _strategyList = new List<IStrategy>();

    public StrategyRunnerService()
    { 
    }

    public async Task LoadService()
    {
       string strategy_to_load = "Strategy.json";
       loadedStrategy = await StrategyLoader.LoadStrategy(strategy_to_load);
        _strategyList.Add(loadedStrategy);       

    }

    public async Task<List<NewOrderDTO>> EvaluateStrategy(ModelOrderLogDTO modelOrderLogDataDTO)
    {
        List<NewOrderDTO> newOrders = new List<NewOrderDTO>();

        foreach (var strategy in _strategyList)
        {
            var newOrder = await strategy.OnTraderModelData(modelOrderLogDataDTO);
            newOrders.Add(newOrder);
        }

        return newOrders;
        //return await loadedStrategy.OnTraderModelData(modelOrderLogDataDTO); 
    }

}
