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

public class AnalyticsRepository : IAnalyticsRepository
{

    private readonly OrderManagementDbContext _context;

    public AnalyticsRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddNewTraderScorecard(int userID, int groupID)
    {
        ScoreCard scd = new ScoreCard
        {
            UserID = userID,
            TradeXML = " ",
            GroupID = groupID
        };

        await _context.ScoreCard.AddAsync(scd);
        return userID;
    }

    public async Task UpdateTraderScoreCard(ScoreCard scData)
    {
        //ScoreCard sc = _context.ScoreCard.FirstOrDefault(s => s.UserID == scData.UserID && s.GroupID == scData.GroupID) ?? new ScoreCard { UserID = -13 };

        ScoreCard sc = await GetTraderScoreCard(scData.UserID, scData.GroupID);    

        if (sc != null)
        {
            sc.Trades = scData.Trades;
            sc.Winners = scData.Winners;
            sc.Losers = scData.Losers;
            sc.Longs = scData.Longs;
            sc.Shorts = scData.Shorts;
            sc.NetProfitLong = scData.NetProfitLong;
            sc.NetProfitShort = scData.NetProfitShort;
            sc.GrossProfit = scData.GrossProfit;
            sc.GrossLoss = scData.GrossLoss;
            sc.LargestWinner = scData.LargestWinner;
            sc.LargestLoser = scData.LargestLoser;
            sc.LargestWinningStreak = scData.LargestWinningStreak;
            sc.LargestLosingStreak = scData.LargestLosingStreak;
            sc.UserID = scData.UserID;
            sc.TotalNetProfit = scData.TotalNetProfit;
            sc.TradeXML = scData.TradeXML;
            sc.GroupID = scData.GroupID;
            sc.LastUpdate = DateTime.UtcNow;

            if (scData.Winners > 0)
                sc.WinLossRatio = Math.Round(Convert.ToDouble(scData.Winners) / Convert.ToDouble(scData.Trades), 2);

            if (scData.GrossProfit > 0)
                sc.AveWin = Math.Round((double)scData.GrossProfit / scData.Winners, 2);

            if (scData.GrossLoss < 0)
                sc.AveLoss = Math.Round((double)scData.GrossLoss / scData.Losers, 2);


            sc.AveTradeDuration = scData.AveTradeDuration;
            sc.AveWinDuration = scData.AveWinDuration;
            sc.AveLossDuration = scData.AveLossDuration;
            sc.StdDevAllTrades = scData.StdDevAllTrades;
            sc.StdDevWinTrades = scData.StdDevWinTrades;
            sc.StdDevLossTrades = scData.StdDevLossTrades;
            sc.SharpRatio = scData.SharpRatio;
            sc.SortinoRatio = scData.SortinoRatio;
            sc.PNL_Last3 = scData.PNL_Last3;
            sc.PNL_Last5 = scData.PNL_Last5;
            sc.PNL_Last8 = scData.PNL_Last8;
            sc.PNL_Last13 = scData.PNL_Last13;
            sc.PNL_Last21 = scData.PNL_Last21;
            sc.PNL_Last34 = scData.PNL_Last34;

            //Console.WriteLine("Updating ScoreCard " + sc.UserID + " " + scData.GroupID + "  " + scData.Winners + "  " + scData.Losers   );

            _context.ScoreCard.Update(sc);
        }

        await Task.CompletedTask;
    }


    public async Task<ScoreCard> GetTraderScoreCard(int UserID, int GroupNumber)
    {
        ScoreCard scoreCard = new ScoreCard();
        var sql = $"SELECT * FROM \"ScoreCard\" WHERE \"UserID\" = {UserID} AND \"GroupID\" = {GroupNumber}"; 

        /*
        scoreCard = await _context.ScoreCard
            .AsNoTracking()
            .Where(s => s.UserID == UserID && s.GroupID == GroupNumber)
            .FirstOrDefaultAsync();
        */

        
        using (NpgsqlConnection connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            scoreCard = await connection.QueryFirstOrDefaultAsync<ScoreCard>(sql);
        }
        return scoreCard;
    }

    public Task ReRankGroupAsync(int groupNumber)
    {
        throw new NotImplementedException();
    }

    public async Task AddModelOrderLogEntry(ModelOrderLog modelOrderLogEntry)
    {
        await _context.ModelOrderLog.AddAsync(modelOrderLogEntry);
    }
}
