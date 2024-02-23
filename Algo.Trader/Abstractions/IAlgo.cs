using OMS.Core.Models;


namespace Algo.Trader.Abstractions;


public interface IAlgo
{
   public NewOrder GenerateAlgoOrder(LiveOrder order);
}
