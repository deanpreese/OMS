using Algo.Trader.Models;
using OMS.Core.Models;
using OMS.Grains.Interfaces;

namespace Algo.Trader.Abstractions;

public abstract class AbstractBase
{
    public UserProfile? _userProfile;
    public ScoreCard? _scoreCard;
    public IGrainFactory _grainFactory;
    public AlgoData _algoData;
    public string trader_key = "";

    public AbstractBase(IGrainFactory grainFactory, AlgoData algoData)
    {
        _grainFactory = grainFactory;
        _algoData = algoData;
    }

    public async void Init(LiveOrder newOrder)
    {
        trader_key = newOrder.UserID + "_" + newOrder.UserGroup;
        ITraderGrain trader =  _grainFactory.GetGrain<ITraderGrain>(trader_key);
        await trader.UpdateProfile(trader_key);
        await trader.UpdateScoreCard(trader_key);
        _userProfile = await trader.GetProfileAsync(trader_key);
        _scoreCard = await trader.GetScoreCardAsync(trader_key);
        Console.WriteLine(" --> " + _algoData.algoname + " : " + trader_key + "  " + newOrder.OrderPX + "  " + newOrder.OrderAction + "  " + _scoreCard.TotalNetProfit );


    }   

}
