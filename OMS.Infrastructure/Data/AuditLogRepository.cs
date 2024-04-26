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

    public async Task AddToClosedTradeLogAsync(ClosedTrade log)
    {

        ClosedTradeLog closedTradeLog = new ClosedTradeLog();
        closedTradeLog.CloseAuthToken = log.CloseAuthToken;  
        closedTradeLog.ClosedOrderMangerID = log.ClosedOrderMangerID;
        closedTradeLog.CloseExecutionID = log.CloseExecutionID;
        closedTradeLog.CloseOrderAction = log.CloseOrderAction;
        closedTradeLog.CloseOrderPX = log.CloseOrderPX;
        closedTradeLog.CloseRelatedOrderID = log.CloseRelatedOrderID;
        closedTradeLog.CloseOrderTime = log.CloseOrderTime;
        closedTradeLog.CloseOrderType  = log.CloseOrderType;
        closedTradeLog.ClosePlatformOrderID = log.ClosePlatformOrderID;
        closedTradeLog.CloseFeatureData = log.CloseFeatureData;

        closedTradeLog.GroupID   = log.GroupID;
        closedTradeLog.Instrument = log.Instrument;
        closedTradeLog.Leverage = log.Leverage;
        closedTradeLog.OpenAuthToken = log.OpenAuthToken;
        closedTradeLog.OpenExecutionID = log.OpenExecutionID;
        closedTradeLog.OpenOrderAction = log.OpenOrderAction;
        closedTradeLog.OpenOrderPX = log.OpenOrderPX;
        closedTradeLog.OpenRelatedOrderID = log.OpenRelatedOrderID;
        closedTradeLog.OppositeTrader = log.OppositeTrader;
        closedTradeLog.OpenPlatformOrderID = log.OpenPlatformOrderID;
        closedTradeLog.OpenOrderMangerID = log.OpenOrderMangerID;
        closedTradeLog.OpenOrderTime = log.OpenOrderTime;
        closedTradeLog.OpenOrderType = log.OpenOrderType;
        closedTradeLog.OpenLiveOrderID = log.OpenLiveOrderID;
        closedTradeLog.OpenFeatureData = log.OpenFeatureData;
        closedTradeLog.Quantity = log.Quantity;
        closedTradeLog.StorerID = log.StorerID;
        closedTradeLog.UserID = log.UserID;

        closedTradeLog.PNL  = log.PNL;
        closedTradeLog.NetChange = log.NetChange;

        await _context.ClosedTradeLog.AddAsync(closedTradeLog);
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
            ordFlow.Instrument = orderToAdd.Instrument ?? PlatformConstants.INVALID_SYMBOL;
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
            ordFlow.ModelFeatureData = orderToAdd.ModelFeatureData;
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
