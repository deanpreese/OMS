using OMS.Core.Common;
using OMS.Core.Models;
using OMS.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OMS.Data.Repositories;

public class UnderCoverRepository : IUnderCoverRepository
{
    private readonly OrderManagementDbContext _context;

    public UnderCoverRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public Task AddToActivityLog(ActivityLog log)
    {
        throw new NotImplementedException();
    }

    public Task AddToOrderFlowAsync(LiveOrder orderToAdd)
    {
            OrderFlow ordFlow = new OrderFlow();
            ordFlow.OrderManagerID = orderToAdd.OrderManagerID;
            ordFlow.GroupID = orderToAdd.UserGroup;
            ordFlow.AuthToken = orderToAdd.AuthToken;
            ordFlow.ExecutedOrderID = orderToAdd.ExecutedOrderID;
            ordFlow.Instrument = orderToAdd.Instrument ?? PlatformConstants.InvalidSymbol;
            ordFlow.Leverage = orderToAdd.Leverage;
            ordFlow.Opposite = orderToAdd.Opposite;
            ordFlow.OrderAction = orderToAdd.OrderAction;
            ordFlow.OrderAction = orderToAdd.OrderAction;
            ordFlow.OrderPX = orderToAdd.OrderPX;
            ordFlow.OrderTime = orderToAdd.OrderTime;
            ordFlow.OrderType = orderToAdd.OrderType;
            ordFlow.PlatformOrderID = orderToAdd.PlatformOrderID;
            ordFlow.Quantity = orderToAdd.Quantity;
            ordFlow.RelatedOrderID = orderToAdd.RelatedOrderID;
            ordFlow.UserID = orderToAdd.UserID;

            //ordFlow.ScoreCardData = DAL.DataAccess.StatisticsScorecardHelper.SerializeScorecard(ots.TraderID, ots.OrderType);
            //ordFlow.ProfileData = DAL.DataAccess.UserProfileHelper.SerializeProfile(ots.TraderID, ots.OrderType);


            _context.OrderFlow.Add(ordFlow);
            return Task.CompletedTask;
    }
}
