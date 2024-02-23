using Algo.Trader.Abstractions;
using Algo.Trader.Models;
using OMS.Core.Common;
using OMS.Core.Models;

namespace Algo.Trader.Trader;

public class SimpleOpenClose : AbstractBase, IAlgo 
{
    public SimpleOpenClose(IGrainFactory grainFactory, AlgoData algoData) : base(grainFactory, algoData)
    {
    }

    public NewOrder GenerateAlgoOrder(LiveOrder order)
    {
        Init(order);
        NewOrder n_o = OrderMapping.MapOrder(order);
        n_o.UserGroup = _algoData.group;
        n_o.UserID = _algoData.algo_traderId;
        
        return n_o;

    }
}
