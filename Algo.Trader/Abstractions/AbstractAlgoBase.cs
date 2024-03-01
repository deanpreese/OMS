using Algo.Trader.Models;
using OMS.Core.Models;
using OMS.Grains.Interfaces;

namespace Algo.Trader.Abstractions;

public abstract class AbstractBase
{
    public AlgoData _algoData;
    public IGrainFactory _grainFactory;
    ITraderGrain _trader;
    IAlgoGrain _algoGrain;
    
    public string trader_key = "";

    public UserProfile _traderUserProfile;
    public ScoreCard _traderScoreCard;
    public int _traderOpenOrdersCount = 0;
    public ClosedTrade _traderLastClosedTrade;

    public string _algo_key = "";
    public List<LiveOrder> AlgoLiveOrders = new List<LiveOrder>();
    public List<IAlgoFilter> _filters = new List<IAlgoFilter>();


    public AbstractBase(IGrainFactory grainFactory, AlgoData algoData)
    {
        _grainFactory = grainFactory;
        _algoData = algoData;
        _algo_key = _algoData.algo_traderId + "_" + _algoData.group;
    }

    public async void InitializeGrains(string trader_key)
    {
        _trader =  _grainFactory.GetGrain<ITraderGrain>(trader_key);
        _traderUserProfile = await _trader.GetProfileAsync(trader_key);
        _traderScoreCard = await _trader.GetScoreCardAsync(trader_key);
        _traderOpenOrdersCount = await _trader.GetLiveOrderCount();
        _traderLastClosedTrade = await _trader.GetLastClosedTrade(trader_key);

        _algoGrain = _grainFactory.GetGrain<IAlgoGrain>(_algo_key);
        AlgoLiveOrders = await _algoGrain.GetLiveOrders(trader_key);

    }   

    public abstract int CheckFilters();
    public abstract LiveOrder DetermineAlgoAction(LiveOrder order);
    
}
