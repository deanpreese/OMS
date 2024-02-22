using Algo.Algorithms.Abstractions;
using Algo.Algorithms.Models;
using OMS.Core.Models;

namespace Algo.Algorithms.Algos;

public class SimpleOpenClose : AbstractBase, IAlgo 
{
    public SimpleOpenClose(UserProfile userProfile, ScoreCard scoreCard, AlgoData algoData) : base(userProfile, scoreCard, algoData)
    {
    }

    public void GenerateAlgoOrder(LiveOrder order)
    {
    }
}
