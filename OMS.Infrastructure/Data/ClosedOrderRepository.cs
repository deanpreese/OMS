using System.Security.Cryptography;
using OMS.Core.Models;
using OMS.Core.Interfaces;
using OMS.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using OMS.Infrastructure.Data;
using System.Data.SqlClient;
using Dapper;
using Npgsql;

namespace OMS.Infrastructure.Data;

public class ClosedOrderRepository : IClosedOrderRepository
{
    private readonly OrderManagementDbContext _context;

    public ClosedOrderRepository(OrderManagementDbContext context)
    {
        _context = context;
        
    }

    // *****************************************************************
    public async Task AddClosedOrder(ClosedTrade o)
    {
        await _context.ClosedTrades.AddAsync(o);
        await Task.CompletedTask;
    }

    public async Task<List<ClosedTrade>> GetClosedOrdersByTraderAsync(int UserID, int GroupNumber)
    {
       return await Get_XXX_ClosedOrdersByTrader(UserID, GroupNumber, -1);
    }


    public async Task<List<ClosedTrade>> Get_XXX_ClosedOrdersByTrader(int UserID, int GroupNumber, int numOrders )
    {
        List<ClosedTrade> histOrders = new List<ClosedTrade>();

        int ordToTake = numOrders;
        if (numOrders < 0)
        {
            ordToTake = 1111111111;
        }

        IEnumerable<ClosedTrade> trades = new List<ClosedTrade>();

        var sql = $"SELECT * FROM \"ClosedTrades\" WHERE \"UserID\" = {UserID} AND \"GroupID\" = {GroupNumber} ORDER BY \"CloseOrderTime\" DESC LIMIT {ordToTake}";

        //trades = _context.ClosedTrades
        //    .Where(c => c.UserID == UserID && c.GroupID == GroupNumber)
        //    .OrderByDescending(c => c.CloseOrderTime)
        //    .Take(ordToTake)
        //    .ToList();

        using (NpgsqlConnection connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            trades = await connection.QueryAsync<ClosedTrade>(sql);
        }

        return await Task.FromResult(trades.ToList());

    }

    public async Task<ClosedTrade> GetLastClosedTradeByOpenPlatformID(int UserID, int GroupNumber, int platform_id)
    {
        ClosedTrade closedTrade = new ClosedTrade();
        var sql = $"SELECT * FROM \"ClosedTrades\" WHERE \"UserID\" = {UserID} AND \"GroupID\" = {GroupNumber} AND \"OpenPlatformOrderID\" = {platform_id} ORDER BY \"CloseOrderTime\" DESC";

        //var closedTrade =  _context.ClosedTrades
        //                    .Where(c=> c.UserID == UserID && c.GroupID == GroupNumber && c.OpenPlatformOrderID == platform_id)
        //                    .OrderByDescending(x => x.CloseOrderTime)
         //                   .AsNoTracking()
         //                   .FirstOrDefault();

        using (NpgsqlConnection connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            closedTrade = await connection.QueryFirstOrDefaultAsync<ClosedTrade>(sql);
        }
        return closedTrade;

    }
}
