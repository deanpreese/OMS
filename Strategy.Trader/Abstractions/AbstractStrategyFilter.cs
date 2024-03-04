using OMS.Core.Models;

namespace Strategy.Trader.Abstractions;

public abstract class AbstractStrategyFilter
{
    public UserProfile _userProfile;
    public ScoreCard _scoreCard;

    public AbstractStrategyFilter(UserProfile userProfile, ScoreCard scoreCard)
    {
        _userProfile = userProfile;
        _scoreCard = scoreCard;    
    }    

}
