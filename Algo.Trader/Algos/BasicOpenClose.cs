using Algo.Trader.Abstractions;
using Algo.Trader.Filters;
using Algo.Trader.Models;
using OMS.Core.Models;
namespace Algo.Trader;

public class BasicOpenClose : AbstractAlgoBase
{
    public BasicOpenClose(IGrainFactory grainFactory, AlgoData algoData) : base(grainFactory, algoData)
    {
       
    }

    public override List<IAlgoFilter> AddFilters(UserProfile userProfile, ScoreCard scoreCard)
    {
        List<IAlgoFilter> _filters = new List<IAlgoFilter>
        {
            new NewAlgoFilter(userProfile, scoreCard)
        };
        return _filters;
    }

    public override int CheckFilters()
    {
        int includeExclude = 0;
        foreach (IAlgoFilter filter in _filters)
        {
            includeExclude = filter.IsInAlgoFilter();
        }
        return includeExclude;
    }
}
