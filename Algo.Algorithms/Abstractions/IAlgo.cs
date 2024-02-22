using OMS.Core.Models;


namespace Algo.Algorithms.Abstractions;


public interface IAlgo
{
   public void GenerateAlgoOrder(LiveOrder order);
}
