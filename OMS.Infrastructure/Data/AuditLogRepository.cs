using OMS.Application.Common;
using OMS.Application.Models;
using OMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;


namespace OMS.Infrastructure.Data;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly OrderManagementDbContext _context;

    public AuditLogRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public Task AddToActivityLog(ActivityLog log)
    {
        throw new NotImplementedException();
    }

    public Task AddToScoreCardLog(ScoreCard scoreCardToAdd)
    {
        ScoreCardLog scFlow = new ScoreCardLog();
            scFlow.Trades = scoreCardToAdd.Trades;
            scFlow.Winners = scoreCardToAdd.Winners;
            scFlow.Losers = scoreCardToAdd.Losers;
            scFlow.Longs = scoreCardToAdd.Longs;
            scFlow.Shorts = scoreCardToAdd.Shorts;
            scFlow.NetProfitLong = scoreCardToAdd.NetProfitLong;
            scFlow.NetProfitShort = scoreCardToAdd.NetProfitShort;
            scFlow.GrossProfit = scoreCardToAdd.GrossProfit;
            scFlow.GrossLoss = scoreCardToAdd.GrossLoss;
            scFlow.LargestWinner = scoreCardToAdd.LargestWinner;
            scFlow.LargestLoser = scoreCardToAdd.LargestLoser;
            scFlow.LargestWinningStreak = scoreCardToAdd.LargestWinningStreak;
            scFlow.LargestLosingStreak = scoreCardToAdd.LargestLosingStreak;
            scFlow.UserID = scoreCardToAdd.UserID;
            scFlow.TotalNetProfit = scoreCardToAdd.TotalNetProfit;
            scFlow.GroupID = scoreCardToAdd.GroupID;
            scFlow.LastUpdate = DateTime.UtcNow;
            scFlow.WinLossRatio = scoreCardToAdd.Winners;
            scFlow.AveWin = scoreCardToAdd.AveWin;
            scFlow.AveLoss = scoreCardToAdd.GrossLoss ;
            scFlow.AveTradeDuration = scoreCardToAdd.AveTradeDuration;
            scFlow.AveWinDuration = scoreCardToAdd.AveWinDuration;
            scFlow.AveLossDuration = scoreCardToAdd.AveLossDuration;

            scFlow.StdDevAllTrades = scoreCardToAdd.StdDevAllTrades;
            scFlow.StdDevWinTrades = scoreCardToAdd.StdDevWinTrades;
            scFlow.StdDevLossTrades = scoreCardToAdd.StdDevLossTrades;
            scFlow.SharpRatio = scoreCardToAdd.SharpRatio;
            scFlow.SortinoRatio = scoreCardToAdd.SortinoRatio;
            
            scFlow.PNL_Last3 = scoreCardToAdd.PNL_Last3;
            scFlow.PNL_Last5 = scoreCardToAdd.PNL_Last5;
            scFlow.PNL_Last8 = scoreCardToAdd.PNL_Last8;
            scFlow.PNL_Last13 = scoreCardToAdd.PNL_Last13;
            scFlow.PNL_Last21 = scoreCardToAdd.PNL_Last21;
            scFlow.PNL_Last34 = scoreCardToAdd.PNL_Last34;
        
        _context.ScoreCardLog.Add(scFlow);
        return Task.CompletedTask;
        
        
    }


    public Task AddToOrderLog(LiveOrder orderToAdd)
    {
            OrderLog ordFlow = new OrderLog();
            ordFlow.OrderManagerID = orderToAdd.OrderManagerID;
            ordFlow.GroupID = orderToAdd.GroupID;
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


            _context.OrderLog.Add(ordFlow);
            return Task.CompletedTask;
    }

    public async Task AddModelOrderLogEntry(ModelOrderLog modelOrderLogEntry)
    {
        await _context.ModelOrderLog.AddAsync(modelOrderLogEntry);
    }
}
