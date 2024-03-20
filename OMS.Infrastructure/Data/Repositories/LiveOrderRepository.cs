using System.Security.Cryptography;
using OMS.Core.Models;
using OMS.Core.Interfaces;
using OMS.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;


namespace OMS.Infrastructure.Data.Repositories;

public class LiveOrderRepository : ILiveOrderRepository
{
    private readonly OrderManagementDbContext _context;

    public LiveOrderRepository(OrderManagementDbContext context)
    {
        _context = context;
        
    }

    // *****************************************************************
    public async Task AddLiveOrderAsync(LiveOrder o)
    {
        await _context.LiveOrder.AddAsync(o);
        return ;
    }

    public async Task DeleteOrderAsyncByOrderManagerID(int orderManagerID)
    {
        List<LiveOrder> orders = (from o in _context.LiveOrder
                                  where o.OrderManagerID == orderManagerID
                                  select o).ToList();

        foreach (LiveOrder ord in orders)
        {
            _context.LiveOrder.Remove(ord);
        }

        await Task.CompletedTask;
    }

    // *****************************************************************
    public async Task<List<LiveOrder>> GetLiveOrders(int GroupNumber)
    {
                            
        List<LiveOrder> orders = (from o in _context.LiveOrder
                            where o.GroupID == GroupNumber
                            select o).ToList();

         return await Task.FromResult(orders);
    }

    public async Task<List<LiveOrder>> GetOrdersByTraderAsync(int UserID, int GroupNumber)
    {
        /*
        var result1 = await _context.LiveOrder
            .AsNoTracking()
            .Where(c => c.UserID == UserID && c.GroupID == GroupNumber)
            .OrderBy(c => c.OrderTime)
            .ToListAsync();
        */

        // Using FromSqlInterpolated
        var result3 = await _context.LiveOrder
            .FromSqlInterpolated($"SELECT * FROM \"LiveOrder\" WHERE \"UserID\" = {UserID} AND \"GroupID\" = {GroupNumber} ORDER BY \"OrderTime\"")
            .AsNoTracking()
            .ToListAsync();

        return result3;

    }


    public async Task<List<LiveOrder>> GetOrdersByTrader(int UserID, string instrument)
    {
        return await _context.LiveOrder
            .AsNoTracking()
            .Where(c => c.Instrument == instrument && c.UserID == UserID)
            .OrderBy(c => c.OrderTime)
            .ToListAsync();
        
    }

}