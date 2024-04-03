using System.Security.Cryptography;
using OMS.Application.Models;
using OMS.Application.Interfaces;
using OMS.Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System.Data.SqlClient;
using Dapper;
using Npgsql;

namespace OMS.Infrastructure.Data;

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
        await _context.LiveOrders.AddAsync(o);
        return ;
    }

    public async Task DeleteOrderAsyncByOrderManagerID(int orderManagerID)
    {
        List<LiveOrder> orders = (from o in _context.LiveOrders
                                  where o.OrderManagerID == orderManagerID
                                  select o).ToList();

        foreach (LiveOrder ord in orders)
        {
            _context.LiveOrders.Remove(ord);
        }

        await Task.CompletedTask;
    }

    // *****************************************************************
    public async Task<List<LiveOrder>> GetLiveOrders(int GroupNumber)
    {
                            
        List<LiveOrder> orders = (from o in _context.LiveOrders
                            where o.GroupID == GroupNumber
                            select o).ToList();

         return await Task.FromResult(orders);
    }

    public async Task<List<LiveOrder>> GetOrdersByTraderAsync(int UserID, int GroupNumber)
    {
        IEnumerable<LiveOrder> orders = new List<LiveOrder>();
        var sql = $"SELECT * FROM \"LiveOrders\" WHERE \"UserID\" = {UserID} AND \"GroupID\" = {GroupNumber} ORDER BY \"OrderTime\"";

        //var result = await _context.LiveOrder
        //    .AsNoTracking()
        //    .Where(c => c.UserID == UserID && c.GroupID == GroupNumber)
        //    .OrderBy(c => c.OrderTime)
        //    .ToListAsync();

        /*
        var result = await _context.LiveOrder
            //.FromSqlInterpolated()
           .FromSqlRaw(sql)
            .AsNoTracking()
            .ToListAsync();

            return result.ToList();
        */

        using (NpgsqlConnection connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            orders = await connection.QueryAsync<LiveOrder>(sql);
        }

        return orders.ToList();

    }


    public async Task<List<LiveOrder>> GetOrdersByTrader(int UserID, string instrument)
    {

        IEnumerable<LiveOrder> orders = new List<LiveOrder>();
        var sql = $"SELECT * FROM \"LiveOrders\" WHERE \"UserID\" = {UserID} AND \"Instrument\" = \'{instrument}\' ORDER BY \"OrderTime\"";

        /*
        orders = await _context.LiveOrder
            .AsNoTracking()
            .Where(c => c.Instrument == instrument && c.UserID == UserID)
            .OrderBy(c => c.OrderTime)
            .ToListAsync();
        
        
        */

        using (NpgsqlConnection connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            orders = await connection.QueryAsync<LiveOrder>(sql);
        }
        
        return orders.ToList();

    }

}