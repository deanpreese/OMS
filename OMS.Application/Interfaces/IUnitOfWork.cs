namespace OMS.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ILiveOrderRepository LiveOrderRepository { get; }
    IClosedOrderRepository ClosedOrderRepository { get; }
    ITraderRepository TraderRepository { get; }
    IAuditLogRepository AuditLogRepository { get; }
    IAnalyticsRepository AnalyticsRepository { get; }
    Task CommitAsync();
    void Commit();
}