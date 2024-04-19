using OMS.Application.Models;

namespace OMS.Application.Interfaces;

public interface IAuditLogRepository
{
    Task AddToOrderLog(LiveOrder order);
    Task AddToActivityLog(ActivityLog logEntry);
    Task AddModelOrderLogEntry(ModelOrderLog modelOrderLogEntry);
    Task AddToScoreCardLog(ScoreCard scoreCardToAdd);
    Task AddToClosedTradeLogAsync(ClosedTrade log);
}
