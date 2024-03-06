

using OMS.Core.Models;
using Strategy.Trader.Abstractions;

namespace Strategy.Trader.Filters;

public class AllFollowFilter : IStrategyFilter
{
    UserProfile _userProfile;
    ScoreCard _scoreCard;

    public void UpdateFilter(UserProfile userProfile, ScoreCard scoreCard)
    {
        _userProfile = userProfile; 
        _scoreCard = scoreCard;   
    }


    public int IsInFilter()
    {
        return 1;
    }

}
