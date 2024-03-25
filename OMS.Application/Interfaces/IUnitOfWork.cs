namespace OMS.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ILiveOrderRepository LiveOrderRepository { get; }
    IClosedOrderRepository ClosedOrderRepository { get; }
    ITraderRepository TraderRepository { get; }
    IUnderCoverRepository UnderCoverRepository { get; }
    IAnalyticsRepository AnalyticsRepository { get; }
    Task CommitAsync();
    void Commit();
}