using OMS.Application.Models;

namespace OMS.Application.Interfaces;

public interface IAnalyticsRepository
{
    public Task UpdateTraderScoreCard(ScoreCard scoreCard);
    public Task<ScoreCard> GetTraderScoreCard(int UserID, int GroupNumber);
    public Task ReRankGroupAsync(int groupNumber);

}
