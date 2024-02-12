namespace OMS.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IOrderRepository OrderRepository { get; }
    ITraderRepository TraderRepository { get; }
    IUnderCoverRepository UnderCoverRepository { get; }
    IAnalyticsRepository AnalyticsRepository { get; }
    Task CommitAsync();
    void Commit();
}