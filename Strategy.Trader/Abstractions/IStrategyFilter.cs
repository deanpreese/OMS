
using OMS.Core.Models;


namespace Strategy.Trader.Abstractions;


public interface IStrategyFilter
{
    public int IsInFilter(ScoreCard scoreCard);
}
