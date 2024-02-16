using System.Data.Entity.Core.Metadata.Edm;
using OMS.Core.Interfaces;
using OMS.Core.Models;
using SQLitePCL;

namespace OMS.Data;

public class AnalyticsRepository : IAnalyticsRepository
{

    private readonly OrderManagementDbContext _context;

    public AnalyticsRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public Task AddTraderScoreCard(ScoreCard scoreCard)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateTraderScoreCard(ScoreCard scData)
    {
        ScoreCard sc = _context.ScoreCard.FirstOrDefault(s => s.UserID == scData.UserID && s.GroupID == scData.GroupID) ?? new ScoreCard { UserID = -13 };

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
                sc.UserID= scData.UserID;
                sc.TotalNetProfit = scData.TotalNetProfit;
                sc.TradeXML = scData.TradeXML;
                sc.GroupID = scData.GroupID;
                sc.LastUpdate = DateTime.UtcNow;

                if (scData.Winners > 0)
                    sc.WinLossRatio = Math.Round((Convert.ToDouble(scData.Winners) / Convert.ToDouble(scData.Trades)), 2);

                if (scData.GrossProfit > 0)
                    sc.AveWin = Math.Round((double)scData.GrossProfit / (double)scData.Winners, 2);

                if (scData.GrossLoss < 0)
                    sc.AveLoss = Math.Round((double)scData.GrossLoss / (double)scData.Losers, 2);

            _context.ScoreCard.Update(sc);
        }

       await Task.CompletedTask; 
    }


    public async Task<ScoreCard> GetTraderScoreCard(int UserID, int GroupNumber)
    {
        var scoreCard =  _context.ScoreCard.FirstOrDefault(s => s.UserID == UserID && s.GroupID == GroupNumber) ?? new ScoreCard { UserID = -13 };
        await Task.FromResult(scoreCard);
        return scoreCard;
    }

    public Task ReRankGroupAsync(int groupNumber)
    {
        throw new NotImplementedException();
    }


}
