using System.Data.Entity;
using System.Security.Cryptography;
using OMS.Core.Models;
using OMS.Core.Interfaces;
using OMS.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace OMS.Infrastructure.Data.Repositories;

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

    public async Task<ClosedTrade> GetLastClosedTrade(int UserID, int GroupNumber)
    {
        ClosedTrade closedTrade = new ClosedTrade();


        List<ClosedTrade> trades = ((from o in _context.ClosedTrades
                                   where o.UserID == UserID 
                                    && o.GroupID == GroupNumber 
                                   select o)).ToList();
        if (trades.Count() > 0)
        {
            closedTrade = trades.Last();
        }
    
        return await Task.FromResult(closedTrade);

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

        List<ClosedTrade> orders = new List<ClosedTrade>();
        
        orders = (from o in _context.ClosedTrades
                      where o.UserID == UserID
                      && o.GroupID == GroupNumber
                      select o).OrderByDescending(x => x.CloseOrderTime).Take(ordToTake).ToList();

        return await Task.FromResult(orders);

    }

    public async Task<ClosedTrade> GetLastClosedTradeByOpenPlatformID(int UserID, int GroupNumber, int platform_id)
    {
        ClosedTrade trade = (from o in _context.ClosedTrades
                      where o.UserID == UserID
                      && o.GroupID == GroupNumber
                      && o.OpenPlatformOrderID == platform_id
                      select o).FirstOrDefault();

        return await Task.FromResult(trade);
    }
}
