using OMS.Core.Models;

namespace OMS.Core.Interfaces;

public interface IAnalyticsRepository
{
    public Task AddTraderScoreCard(ScoreCard scoreCard);
    public Task UpdateTraderScoreCard(ScoreCard scoreCard);
    public Task<ScoreCard> GetTraderScoreCard(int UserID, int GroupNumber);
    public Task<List<double>> GetDataByTraderSparklineAsync(int UserID, int GroupNumber);
    public Task ReRankGroupAsync(int groupNumber);

}
