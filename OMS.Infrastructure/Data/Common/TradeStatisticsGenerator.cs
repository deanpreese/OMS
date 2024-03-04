using System.Xml.Serialization;
using OMS.Core.Common;
using OMS.Core.Models;

namespace OMS.Infrastructure.Data.Common;

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

        scoreCard.AveTradeDuration = closedTrades.Average(trade => (trade.CloseOrderTime - trade.OpenOrderTime).TotalSeconds);

        // Calculate average time in winners
        var winners_list = closedTrades.Where(trade => trade.PNL > 0);
        scoreCard.AveWinDuration = winners_list.Any() ? winners_list.Average(trade => (trade.CloseOrderTime - trade.OpenOrderTime).TotalSeconds) : 0;

        // Calculate average time in losers
        var losers_list = closedTrades.Where(trade => trade.PNL <= 0);
        scoreCard.AveLossDuration = losers_list.Any() ? losers_list.Average(trade => (trade.CloseOrderTime - trade.OpenOrderTime).TotalSeconds) : 0;

        // Standard Deviation of P&L
        double meanProfit = closedTrades.Average(trade => trade.PNL);
        double variance = closedTrades.Sum(trade => Math.Pow(trade.PNL - meanProfit, 2)) / closedTrades.Count;
        scoreCard.StdDevAllTrades = Math.Sqrt(variance);

        // Standard Deviation for Winners
        double meanProfitWinners = winners_list.Any() ? winners_list.Average(trade => trade.PNL) : 0;
        double varianceWinners = winners_list.Sum(trade => Math.Pow(trade.PNL - meanProfitWinners, 2)) / winners_list.Count();
        scoreCard.StdDevWinTrades = Math.Sqrt(varianceWinners);

        // Standard Deviation for Losers
        double meanProfitLosers = losers_list.Any() ? losers_list.Average(trade => trade.PNL) : 0;
        double varianceLosers = losers_list.Sum(trade => Math.Pow(trade.PNL - meanProfitLosers, 2)) / losers_list.Count();
        scoreCard.StdDevLossTrades = Math.Sqrt(varianceLosers);

        // Sharpe Ratio
        double riskFreeRate = 0.01; // Assuming a 1% risk-free rate. Adjust as needed.
        scoreCard.SharpRatio = (meanProfit - riskFreeRate) / scoreCard.StdDevAllTrades;

        // Sortino Ratio
        var negativeProfits = closedTrades.Where(trade => trade.PNL < 0).Select(trade => trade.PNL).ToList();
        double meanNegativeProfits = negativeProfits.Any() ? negativeProfits.Average() : 0;
        double downsideVariance = negativeProfits.Sum(profit => Math.Pow(profit - meanNegativeProfits, 2)) / negativeProfits.Count;
        double downsideDeviation = Math.Sqrt(downsideVariance);
        scoreCard.SortinoRatio = (meanProfit - riskFreeRate) / downsideDeviation;


        scoreCard.PNL_Last3 = CalculatePNLByDuration(closedTrades, 3);
        scoreCard.PNL_Last5 = CalculatePNLByDuration(closedTrades, 5);
        scoreCard.PNL_Last8 = CalculatePNLByDuration(closedTrades, 8);
        scoreCard.PNL_Last13 = CalculatePNLByDuration(closedTrades, 13);
        scoreCard.PNL_Last21 = CalculatePNLByDuration(closedTrades, 21);
        scoreCard.PNL_Last34 = CalculatePNLByDuration(closedTrades, 34);

        return scoreCard;
    }

    private static double CalculatePNLByDuration(List<ClosedTrade> trades, int lastN)
    {
        double rtn_val = 0;
        if (trades.Count > lastN)
        {
            var lastTrades = trades.TakeLast(lastN); // Get the last N trades efficiently
            return lastTrades.Sum(trade => trade.PNL);
        }
        return rtn_val;

    }

}
