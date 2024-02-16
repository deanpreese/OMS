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
        }

        scoreCard.LargestWinningStreak = largestWinningStreak;
        scoreCard.LargestLosingStreak = largestLosingStreak;

        // Example serialization of trades to XML for TradeXML property
        // This requires implementing serialization logic based on your needs
        scoreCard.TradeXML = SerializeTradesToXml(closedTrades);

        return scoreCard;
    }

    private static string SerializeTradesToXml(List<ClosedTrade> trades)
    {
        // Implement XML serialization of trades here
        // This is a placeholder for actual serialization logic
        return "<Trades>...</Trades>";
    }
}
