using System.Security.Cryptography;
using OMS.Application.Models;
using OMS.Application.Interfaces;
using OMS.Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System.Data.SqlClient;
using Dapper;
using Npgsql;
using Microsoft.EntityFrameworkCore.Storage.Json;

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

            sc.StdDevAllTrades = Math.Round(scData.StdDevAllTrades, 6) ;
            sc.StdDevWinTrades = Math.Round(scData.StdDevWinTrades, 6) ;
            sc.StdDevLossTrades = Math.Round(scData.StdDevLossTrades, 6);
            sc.SharpRatio = Math.Round(scData.SharpRatio, 6);
            sc.SortinoRatio = Math.Round(scData.SortinoRatio, 6) ;

            sc.PNL_Last3 = scData.PNL_Last3;
            sc.PNL_Last5 = scData.PNL_Last5;
            sc.PNL_Last8 = scData.PNL_Last8;
            sc.PNL_Last13 = scData.PNL_Last13;
            sc.PNL_Last21 = scData.PNL_Last21;
            sc.PNL_Last34 = scData.PNL_Last34;

            if (scData.Trades > 0)    
            {
                sc.Rank = await GetTraderRank(scData.UserID, scData.GroupID);
                sc.SortinoRank = await GetSortinoRank(scData.UserID, scData.GroupID);
                sc.SharpeRank = await GetSharpeRank(scData.UserID, scData.GroupID);    
            }else
            {
                sc.Rank = 0;
                sc.SortinoRank = 0;
                sc.SharpeRank = 0;
            }

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

    public async Task<int> GetTraderRank(int userID, int groupNumber)
    {
        int rank = 0;

        string query = @"
            WITH RankedUsers AS (
                SELECT
                    SC.""UserID"",
                    SC.""TotalNetProfit"",
                    RANK() OVER (ORDER BY SC.""TotalNetProfit"" DESC) AS ""PNL_Rank""
                FROM
                    public.""ScoreCards"" as SC
                WHERE
                    SC.""GroupID"" = @GroupNumber
            )
            SELECT
                ""PNL_Rank""
            FROM
                RankedUsers
            WHERE
                ""UserID"" = @UserID;";

        using (NpgsqlConnection connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            rank = await connection.QuerySingleAsync<int>(query, new { UserId = userID, groupNumber }); 
        }

        return rank;

    }


    public async Task<int> GetSortinoRank(int userID, int groupNumber)
    {
        int rank = 0;

        string query = @"
            WITH RankedUsers AS (
                SELECT
                    SC.""UserID"",
                    SC.""SortinoRatio"",
                    RANK() OVER (ORDER BY SC.""SortinoRatio"" DESC) AS ""PNL_Rank""
                FROM
                    public.""ScoreCards""  as SC
                WHERE
                    SC.""GroupID"" = @GroupNumber
            )
            SELECT
                ""PNL_Rank""
            FROM
                RankedUsers
            WHERE
                ""UserID"" = @UserID;";

        using (NpgsqlConnection connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            rank = await connection.QuerySingleAsync<int>(query, new { UserId = userID, groupNumber });   
        }

        return rank;

    }


    public async Task<int> GetSharpeRank(int userID, int groupNumber)
    {
        int rank = 0;

        string query = @"
            WITH RankedUsers AS (
                SELECT
                    SC.""UserID"",
                    SC.""SharpRatio"",
                    RANK() OVER (ORDER BY SC.""SharpRatio"" DESC) AS ""PNL_Rank""
                FROM
                    public.""ScoreCards"" as SC
                WHERE
                    SC.""GroupID"" = @GroupNumber
            )
            SELECT
                ""PNL_Rank""
            FROM
                RankedUsers
            WHERE
                ""UserID"" = @UserID;";

        using (NpgsqlConnection connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            rank = await connection.QuerySingleAsync<int>(query, new { UserId = userID, groupNumber }); 
        }

        return rank;

    }

}
