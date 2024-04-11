using OMS.Application.Models;
using OMS.SharedKernel.DTO;

namespace OMS.Application.Interfaces;

public interface IAnalyticsRepository
{
    public Task UpdateTraderScoreCard(ScoreCard scoreCard);
    public Task<ScoreCard> GetTraderScoreCard(int UserID, int GroupNumber);
    public Task<int> GetTraderRank(int UserID, int groupNumber);

}
