using System;
using System.Collections.Generic;
using System.Threading.Tasks;   
using OMS.Core.Interfaces;

namespace OMS.Infrastructure.Data;
public class UnitOfWork : IUnitOfWork
{
    private readonly OrderManagementDbContext _context;

    public UnitOfWork(OrderManagementDbContext context)
    {
        _context = context;
        LiveOrderRepository = new LiveOrderRepository(_context);
        ClosedOrderRepository = new ClosedOrderRepository(_context);
        TraderRepository = new TraderRepository(_context);
        UnderCoverRepository = new UnderCoverRepository(_context);
        AnalyticsRepository = new AnalyticsRepository(_context);
    }

    public ILiveOrderRepository LiveOrderRepository { get; private set; }
    public IClosedOrderRepository ClosedOrderRepository { get; private set; }
    public ITraderRepository TraderRepository { get; private set; }
    public IUnderCoverRepository UnderCoverRepository { get; private set; }
    public IAnalyticsRepository AnalyticsRepository { get; private set; }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            await _context.SaveChangesAsync();
        }    
     
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