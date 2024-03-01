using OMS.Core.Models;


namespace Algo.Trader.Abstractions;


public interface IAlgo
{
   public Task<NewOrder> GenerateAlgoOrder(LiveOrder order);
}
