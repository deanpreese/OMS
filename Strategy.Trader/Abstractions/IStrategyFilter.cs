using OMS.SharedKernel.DTO;

namespace Strategy.Trader.Abstractions;


public interface IStrategyFilter
{
    public int IsInFilter(ScoreCardDTO scoreCard);
}
