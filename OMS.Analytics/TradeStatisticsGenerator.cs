using System.Xml.Serialization;
using OMS.Core.Common;
using OMS.Core.Models;
using OMS.Data;

namespace OMS.Analytics;

public static class TradeStatisticsGenerator
{
    public static ScoreCard GenerateScoreCard(int userId, int groupId, List<ClosedTrade> closedTrades)
    {
        var scoreCard = new ScoreCard
        {
            UserID = userId,
            GroupID = groupId,
            Trades = closedTrades.Count,
            Winners = closedTrades.Count(trade => trade.PNL > 0),
            Losers = closedTrades.Count(trade => trade.PNL <= 0),
            Longs = closedTrades.Count(trade => trade.OpenOrderAction == OrderAction.Buy),
            Shorts = closedTrades.Count(trade => trade.OpenOrderAction == OrderAction.Sell),
            GrossProfit = closedTrades.Where(trade => trade.PNL > 0).Sum(trade => trade.PNL),
            GrossLoss = closedTrades.Where(trade => trade.PNL <= 0).Sum(trade => trade.PNL),
            LargestWinner = closedTrades.Any(trade => trade.PNL > 0) ? closedTrades.Max(trade => trade.PNL) : 0,
            LargestLoser = closedTrades.Any(trade => trade.PNL < 0) ? closedTrades.Min(trade => trade.PNL) : 0,
            LastUpdate = DateTime.UtcNow
        };

        scoreCard.NetProfitLong = closedTrades
            .Where(trade => trade.OpenOrderAction == OrderAction.Buy && trade.PNL > 0)
            .Sum(trade => trade.PNL);

        scoreCard.NetProfitShort = closedTrades
            .Where(trade => trade.OpenOrderAction == OrderAction.Sell && trade.PNL > 0)
            .Sum(trade => trade.PNL);

        scoreCard.TotalNetProfit = scoreCard.GrossProfit + scoreCard.GrossLoss; // GrossLoss is negative

        if (scoreCard.Winners > 0)
        {
            scoreCard.WinLossRatio = (double)scoreCard.Winners / scoreCard.Losers;
            scoreCard.AveWin = scoreCard.GrossProfit / scoreCard.Winners;
        }

        if (scoreCard.Losers > 0)
        {
            scoreCard.AveLoss = scoreCard.GrossLoss / scoreCard.Losers; // GrossLoss is negative
        }

        // Streak calculations
        int currentWinningStreak = 0, currentLosingStreak = 0, largestWinningStreak = 0, largestLosingStreak = 0;
        foreach (var trade in closedTrades)
        {
            if (trade.PNL > 0)
            {
                currentWinningStreak++;
                currentLosingStreak = 0;
            }
            else if (trade.PNL < 0)
            {
                currentLosingStreak++;
                currentWinningStreak = 0;
            }

            if (currentWinningStreak > largestWinningStreak) largestWinningStreak = currentWinningStreak;
            if (currentLosingStreak > largestLosingStreak) largestLosingStreak = currentLosingStreak;

            /*
            var prevAverageProfitLoss = ave_pnl;
            ave_pnl += (trade.Pnl - ave_pnl) / num_trades;

            sumForVariance += (trade.Pnl - prevAverageProfitLoss) * (trade.Pnl - ave_pnl);
            var variance = num_trades > 1 ? sumForVariance / (num_trades - 1) : 0;
            profitLossStandardDeviation = (double)Math.Sqrt((double)variance);
            */
        }

        
        //double profitFactor = Math.Round( total_net_profit == 0 ? 0 : (gross_loss < 0 ? total_net_profit / Math.Abs(gross_loss) : 10),2);
        //double winRate = Math.Round(num_trades > 0 ? (double)winners / num_trades : 0, 2);
        //double lossRate = Math.Round(num_trades > 0 ? (double)losers / num_trades : 0,2);   


        //double sharpRatio = Math.Round( profitLossStandardDeviation > 0 ? ave_pnl / profitLossStandardDeviation : 0, 2);
        //double sortinoRatio = Math.Round(profitLossStandardDeviation > 0 ? ave_pnl / profitLossStandardDeviation : 0 ,2);
           

        scoreCard.LargestWinningStreak = largestWinningStreak;
        scoreCard.LargestLosingStreak = largestLosingStreak;

        return scoreCard;
    }

}
