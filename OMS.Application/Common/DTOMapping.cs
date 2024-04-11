using System.Reflection.Metadata.Ecma335;
using OMS.Application.Models;

using OMS.SharedKernel.DTO;

namespace OMS.Application.Common;

public class DTOMapping
{
    public static async Task<ClosedTrade> MapClosedOrder(LiveOrder orderToClose, LiveOrder orderToStore)
    {
        ClosedTrade histOrder = new ClosedTrade()
        {
            UserID = orderToClose.UserID,
            GroupID = orderToClose.GroupID,
            Instrument = orderToClose.Instrument,
            Quantity = orderToClose.Quantity,
            Leverage = orderToClose.Leverage,
            OppositeTrader = false,

            OpenLiveOrderID = orderToClose.LiveOrderID,
            OpenPlatformOrderID = orderToClose.PlatformOrderID,
            OpenOrderMangerID = orderToClose.OrderManagerID,
            OpenAuthToken = orderToClose.AuthToken,
            OpenExecutionID = orderToClose.ExecutedOrderID,
            OpenRelatedOrderID = orderToClose.RelatedOrderID,
            OpenOrderTime = orderToClose.OrderTime,
            OpenOrderPX = orderToClose.OrderPX,
            OpenOrderType = orderToClose.OrderType,
            OpenOrderAction = orderToClose.OrderAction,

            ClosePlatformOrderID = orderToStore.PlatformOrderID,
            ClosedOrderMangerID = orderToStore.OrderManagerID,
            CloseAuthToken = orderToStore.AuthToken,
            CloseExecutionID = orderToStore.ExecutedOrderID,
            CloseRelatedOrderID = orderToStore.RelatedOrderID,
            CloseOrderTime = orderToStore.OrderTime,
            CloseOrderPX = orderToStore.OrderPX,
            CloseOrderType = orderToStore.OrderType,
            CloseOrderAction = orderToStore.OrderAction
        };

        return await Task.FromResult(histOrder);
    }


    public static async Task<LiveOrder> MapOrderNewToLive(NewOrderDTO newOrder)
    {
        LiveOrder liveOrder = new LiveOrder();
        liveOrder.OrderPX = newOrder.OrderPX;  
        liveOrder.OrderTime = newOrder.OrderTime;
        liveOrder.OrderManagerID = 0;
        liveOrder.Instrument = newOrder.Instrument;
        liveOrder.OrderAction = newOrder.OrderAction;
        liveOrder.OrderType = newOrder.OrderType;
        liveOrder.PlatformOrderID = newOrder.PlatformOrderID;
        liveOrder.Quantity = newOrder.Quantity;
        liveOrder.UserID = newOrder.UserID;
        liveOrder.GroupID = newOrder.GroupID;
        liveOrder.Leverage = 1;
        liveOrder.Opposite = 0;
        liveOrder.RelatedOrderID = newOrder.RelatedOrderID;

        return await Task.FromResult(liveOrder);
    }





    public async static Task<NewOrderDTO> MapOrderLiveToNew(LiveOrder liveOrder)
    {
        NewOrderDTO newOrder = new NewOrderDTO {
            OrderPX = liveOrder.OrderPX , 
            OrderTime = liveOrder.OrderTime,
            Instrument = liveOrder.Instrument,
            OrderAction = liveOrder.OrderAction,
            OrderType = liveOrder.OrderType,
            PlatformOrderID = 0,
            Quantity = liveOrder.Quantity,
            UserID = 0,
            GroupID = 0,
        };
        return await Task.FromResult(newOrder);

    }



    public static async Task<LiveOrderDTO> MapOrderLiveToLiveDTO(LiveOrder newOrder)
    {
        LiveOrderDTO liveOrderDTO = new LiveOrderDTO();
        liveOrderDTO.OrderPX = newOrder.OrderPX;  
        liveOrderDTO.OrderTime = newOrder.OrderTime;
        liveOrderDTO.OrderManagerID = newOrder.OrderManagerID ;
        liveOrderDTO.Instrument = newOrder.Instrument;
        liveOrderDTO.OrderAction = newOrder.OrderAction;
        liveOrderDTO.OrderType = newOrder.OrderType;
        liveOrderDTO.PlatformOrderID = newOrder.PlatformOrderID;
        liveOrderDTO.Quantity = newOrder.Quantity;
        liveOrderDTO.UserID = newOrder.UserID;
        liveOrderDTO.GroupID = newOrder.GroupID;
        liveOrderDTO.Leverage = 1;
        liveOrderDTO.Opposite = 0;
        liveOrderDTO.RelatedOrderID = newOrder.RelatedOrderID;

        return await Task.FromResult(liveOrderDTO);
    }

    public static async Task<ClosedTradeDTO> MapClosedOrderToClosedOrderDTO(ClosedTrade closedTrade)
    {

        ClosedTradeDTO closedTradeDTO = new ClosedTradeDTO
        {
            StorerID = closedTrade.StorerID,
            UserID = closedTrade.UserID,
            GroupID = closedTrade.GroupID,
            Instrument = closedTrade.Instrument,
            Quantity = closedTrade.Quantity,
            Leverage = closedTrade.Leverage,
            OppositeTrader = closedTrade.OppositeTrader,
            OpenPlatformOrderID = closedTrade.OpenPlatformOrderID,
            OpenOrderMangerID = closedTrade.OpenOrderMangerID,
            OpenAuthToken = closedTrade.OpenAuthToken,
            OpenExecutionID = closedTrade.OpenExecutionID,
            OpenRelatedOrderID = closedTrade.OpenRelatedOrderID,
            OpenOrderTime = closedTrade.OpenOrderTime,
            OpenOrderPX = closedTrade.OpenOrderPX,
            OpenOrderType = closedTrade.OpenOrderType,
            OpenOrderAction = closedTrade.OpenOrderAction,
            ClosePlatformOrderID = closedTrade.ClosePlatformOrderID,
            ClosedOrderMangerID = closedTrade.ClosedOrderMangerID,
            CloseAuthToken = closedTrade.CloseAuthToken,
            CloseExecutionID = closedTrade.CloseExecutionID,
            CloseRelatedOrderID = closedTrade.CloseRelatedOrderID,
            CloseOrderTime = closedTrade.CloseOrderTime,
            CloseOrderPX = closedTrade.CloseOrderPX,
            CloseOrderType = closedTrade.CloseOrderType,
            CloseOrderAction = closedTrade.CloseOrderAction,
            PNL = closedTrade.PNL,
            MAE = closedTrade.MAE,
            MFE = closedTrade.MFE,
            NetChange = closedTrade.NetChange
        };
        return await Task.FromResult(closedTradeDTO);
    }


    public static async Task<ScoreCardDTO> MapScorecardToScorecardDTO(ScoreCard scoreCard)
    {
        
        ScoreCardDTO scoreCardDTO = new ScoreCardDTO
        {
            UserID = scoreCard.UserID,
            GroupID = scoreCard.GroupID,
            Trades = scoreCard.Trades,
            Winners = scoreCard.Winners,
            Losers = scoreCard.Losers,
            Longs = scoreCard.Longs,
            Shorts = scoreCard.Shorts,
            NetProfitLong = scoreCard.NetProfitLong,
            NetProfitShort = scoreCard.NetProfitShort,
            GrossProfit = scoreCard.GrossProfit,
            GrossLoss = scoreCard.GrossLoss,
            LargestWinner = scoreCard.LargestWinner,
            LargestLoser = scoreCard.LargestLoser,
            TotalNetProfit = scoreCard.TotalNetProfit,
            WinLossRatio = scoreCard.WinLossRatio,
            AveWin = scoreCard.AveWin,
            AveLoss = scoreCard.AveLoss,
            AveLossDuration = scoreCard.AveLossDuration,
            AveTradeDuration = scoreCard.AveTradeDuration,
            AveWinDuration = scoreCard.AveWinDuration,
            StdDevAllTrades = scoreCard.StdDevAllTrades,
            StdDevWinTrades = scoreCard.StdDevWinTrades,
            StdDevLossTrades = scoreCard.StdDevLossTrades,
            SharpRatio = scoreCard.SharpRatio,
            SortinoRatio = scoreCard.SortinoRatio,
            PNL_Last3 = scoreCard.PNL_Last3,
            PNL_Last5 = scoreCard.PNL_Last5,
            PNL_Last8 = scoreCard.PNL_Last8,
            PNL_Last13 = scoreCard.PNL_Last13,
            PNL_Last21 = scoreCard.PNL_Last21,
            PNL_Last34 = scoreCard.PNL_Last34,
            LastUpdate = scoreCard.LastUpdate,
            Rank = scoreCard.Rank,
            SortinoRank = scoreCard.SortinoRank,
            SharpeRank = scoreCard.SharpeRank
        };
        return await Task.FromResult(scoreCardDTO);


    }


}

   