using System.Data.Entity;
using System.Security.Cryptography;
using OMS.Core.Models;
using OMS.Core.Interfaces;
using OMS.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace OMS.Infrastructure.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderManagementDbContext _context;

    public OrderRepository(OrderManagementDbContext context)
    {
        _context = context;
        
    }



    // *****************************************************************
    public async Task AddLiveOrderAsync(LiveOrder o)
    {
        await _context.LiveOrder.AddAsync(o);
        return ;
    }

    public Task<int> UpdateOrder(LiveOrder o)
    {
        throw new NotImplementedException();
    }

    public Task<List<LiveOrder>> UpdateOrderPriceByExeutionIDAsync(int executionId, double price)
    {
        List<LiveOrder> orders = (from o in _context.LiveOrder
                                where o.OrderManagerID== executionId
                                select o).ToList();

        foreach (LiveOrder ord in orders)
        {
            ord.OrderPX = price;
        }

        return Task.FromResult(orders);
    }

    public Task DeleteOrderAsyncByRTOrderID(int rtOrderID)
    {
        List<LiveOrder> orders = (from o in _context.LiveOrder
                                  where o.RelatedOrderID == rtOrderID
                                  select o).ToList();

        foreach (LiveOrder ord in orders)
        {
            _context.LiveOrder.Remove(ord);
        }

        return Task.CompletedTask;
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


    public async Task<List<LiveOrder>> GetOrdersByExecutionIDAsync(int executionID)
    {
        List<LiveOrder> orders = (from o in _context.LiveOrder
                            where o.ExecutedOrderID == executionID
                            select o).ToList();
         return await Task.FromResult(orders);
    }

    public async Task<List<LiveOrder>> GetOrdersByGroupAsync(int groupNumber)
    {
        List<LiveOrder> orders =(from o in _context.LiveOrder
                                where o.GroupID == groupNumber
                                select o).ToList();
        return await Task.FromResult(orders);
    }

    public async Task<List<LiveOrder>> GetOrdersByInstrumentAsync(string instrument)
    {
        List<LiveOrder> orders = (from o in _context.LiveOrder
                                    where o.Instrument.StartsWith( instrument)
                                select o).ToList();

        return await Task.FromResult(orders);
    }

    public Task<List<NewOrder>> GetOrdersByInstrumentByGroupAsync(string instrument, int group)
    {
        throw new NotImplementedException();
    }

    public async Task<List<LiveOrder>> GetOrdersByPlatformIDAsync(int platformID)
    {
        List<LiveOrder> orders = (from o in _context.LiveOrder
                                where o.PlatformOrderID == platformID
                                select o).ToList();
        return await Task.FromResult(orders);
    }


    public async Task<List<LiveOrder>> GetOrdersByTraderAsync(int UserID, int GroupNumber)
    {
        List<LiveOrder> orders = (from c in _context.LiveOrder
                            where c.UserID == UserID
                            && c.GroupID == GroupNumber
                            orderby c.OrderTime
                            select c).ToList();

        return await Task.FromResult(orders);
    }


    public async Task<List<LiveOrder>> GetOrdersByTrader(int UserID, string instrument)
    {
        List<LiveOrder> orders = (from c in _context.LiveOrder
                                where c.Instrument == instrument 
                                && c.UserID == UserID
                                orderby c.OrderTime
                                select c).ToList();
        return await Task.FromResult(orders);
    }



    public async Task<List<LiveOrder>> GetOrdersByTrader(int UserID, string instrument, OrderAction orderAction)
    {
        List<LiveOrder> orders = (from c in _context.LiveOrder
                                where c.Instrument == instrument 
                                && c.UserID == UserID
                                && c.OrderAction == orderAction
                                orderby c.OrderTime
                                select c).ToList();
        return await Task.FromResult(orders);
    }



    // *****************************************************************
    public async Task AddClosedOrder(ClosedTrade o)
    {
        await _context.ClosedTrades.AddAsync(o);
        await Task.CompletedTask;
    }

    public async Task<List<LiveOrder>> GetOrdersByRelatedOrderIDAsync(int relatedOrderID)
    {
         List<LiveOrder> orders = ((from o in _context.LiveOrder
                                   where o.RelatedOrderID == relatedOrderID
                                   select o)).ToList();

         return await Task.FromResult(orders);                                   
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


    public Task<List<ClosedTrade>> GetClosedTrades(int GroupNumber)
    {
        throw new NotImplementedException();
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
