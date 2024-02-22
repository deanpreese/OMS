using OMS.Core.Models;

namespace Algo.Algorithms;

public abstract class AbstractAlgoFilter
{
    public UserProfile _userProfile;
    public ScoreCard _scoreCard;

    public AbstractAlgoFilter(UserProfile userProfile, ScoreCard scoreCard)
    {
        _userProfile = userProfile;
        _scoreCard = scoreCard;    
    }    

}
