using Algo.Algorithms.Models;
using OMS.Core.Models;

namespace Algo.Algorithms.Abstractions;

public abstract class AbstractBase
{
    public UserProfile _userProfile;
    public ScoreCard _scoreCard;
    public AlgoData _algoData;

    public AbstractBase(UserProfile userProfile, ScoreCard scoreCard, AlgoData algoData)
    {
        _userProfile = userProfile;
        _scoreCard = scoreCard;
        _algoData = algoData;
    }   

}
