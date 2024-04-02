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
            sc.GroupID = scData.GroupID;
            sc.LastUpdate = DateTime.UtcNow;

            sc.WinLossRatio = scData.Trades != 0 ? Math.Round((double)scData.Winners / scData.Trades, 2) : 0;
            sc.AveWin = scData.Winners != 0 ? Math.Round((double)scData.GrossProfit / scData.Winners, 2) : 0;
            sc.AveLoss = scData.Losers != 0 ? Math.Round((double)scData.GrossLoss / scData.Losers, 2) : 0;

            sc.AveTradeDuration = Math.Round(scData.AveTradeDuration, 6);
            sc.AveWinDuration = sc.AveWin > 0 ? Math.Round(scData.AveWinDuration, 6) : 0;
            sc.AveLossDuration = sc.AveLoss > 0 ? Math.Round(scData.AveLossDuration, 6): 0;

            sc.StdDevAllTrades = scData.StdDevAllTrades > 0 ? Math.Round(scData.StdDevAllTrades, 6) : 0;
            sc.StdDevWinTrades = scData.StdDevWinTrades > 0 ? Math.Round(scData.StdDevWinTrades, 6) : 0;
            sc.StdDevLossTrades = scData.StdDevLossTrades > 0 ? Math.Round(scData.StdDevLossTrades, 6) : 0;
            sc.SharpRatio = Double.IsFinite(sc.SharpRatio) ? Math.Round(scData.SharpRatio, 6) : 0;
            sc.SortinoRatio = Double.IsFinite(sc.SortinoRatio) ? Math.Round(scData.SortinoRatio, 6) : 0;

            sc.PNL_Last3 = scData.PNL_Last3;
            sc.PNL_Last5 = scData.PNL_Last5;
            sc.PNL_Last8 = scData.PNL_Last8;
            sc.PNL_Last13 = scData.PNL_Last13;
            sc.PNL_Last21 = scData.PNL_Last21;
            sc.PNL_Last34 = scData.PNL_Last34;

            //Console.WriteLine("Updating ScoreCard " + sc.UserID + " " + scData.GroupID + "  " + scData.Winners + "  " + scData.Losers   );

            _context.ScoreCards.Update(sc);
        }

        await Task.CompletedTask;
    }


    public async Task<ScoreCard> GetTraderScoreCard(int UserID, int GroupNumber)
    {
        ScoreCard scoreCard = new ScoreCard();
        var sql = $"SELECT * FROM \"ScoreCards\" WHERE \"UserID\" = {UserID} AND \"GroupID\" = {GroupNumber}"; 

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
        // TODO  Implement ReRankGroupAsync
        throw new NotImplementedException();
    }


}
