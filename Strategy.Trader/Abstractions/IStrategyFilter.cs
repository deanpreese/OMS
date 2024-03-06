
using OMS.Core.Models;


namespace Strategy.Trader.Abstractions;


public interface IStrategyFilter
{
    public void UpdateFilter(UserProfile userProfile, ScoreCard scoreCard);
    public int IsInFilter();
}
