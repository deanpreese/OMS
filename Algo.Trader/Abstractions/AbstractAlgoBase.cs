using Algo.Trader.Models;
using OMS.Core.Models;
using OMS.Grains.Interfaces;

namespace Algo.Trader.Abstractions;

public abstract class AbstractBase
{
    public AlgoData _algoData;
    public IGrainFactory _grainFactory;
    public ITraderGrain TraderGrain;
    public IAlgoGrain AlgoGrain;
    public string Algo_Key;
    public List<IAlgoFilter> _filters = new List<IAlgoFilter>();


    public AbstractBase(IGrainFactory grainFactory, AlgoData algoData)
    {
        _grainFactory = grainFactory;
        _algoData = algoData;
        Algo_Key = _algoData.algo_traderId + "_" + _algoData.group;
    }

    public async void InitializeGrains(string trader_key)
    {
        try{
            await Task.Run(() =>
            {
                TraderGrain = _grainFactory.GetGrain<ITraderGrain>(trader_key);
                AlgoGrain = _grainFactory.GetGrain<IAlgoGrain>(Algo_Key);
            });

        }catch (Exception ex)
        {
            Console.WriteLine("INIT ERROR " + ex.StackTrace);
        }

        
    }   

    public abstract int CheckFilters();
    public abstract Task<LiveOrder> DetermineAlgoAction(LiveOrder order);
    
}
