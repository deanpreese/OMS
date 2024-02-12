using System;
using System.Collections.Generic;
using System.Threading.Tasks;   
using OMS.Core.Interfaces;

namespace OMS.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrderManagementDbContext _context;

    public UnitOfWork(OrderManagementDbContext context)
    {
        _context = context;
        OrderRepository = new OrderRepository(_context);
        TraderRepository = new TraderRepository(_context);
        UnderCoverRepository = new UnderCoverRepository(_context);
        AnalyticsRepository = new AnalyticsRepository(_context);
    }

    public IOrderRepository OrderRepository { get; private set; }
    public ITraderRepository TraderRepository { get; private set; }
    public IUnderCoverRepository UnderCoverRepository { get; private set; }
    public IAnalyticsRepository AnalyticsRepository { get; private set; }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }


}