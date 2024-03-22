using OMS.Core.Models;

namespace OMS.Core.Interfaces;

public interface IAnalyticsRepository
{
    Task<int> AddNewTraderScorecard(int userID, int groupID);
    public Task UpdateTraderScoreCard(ScoreCard scoreCard);
    public Task<ScoreCard> GetTraderScoreCard(int UserID, int GroupNumber);
    public Task ReRankGroupAsync(int groupNumber);
    public Task AddModelOrderLogEntry(ModelOrderLog modelOrderLogEntry);

}
