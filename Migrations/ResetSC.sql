UPDATE public."ScoreCard"
	SET 
     "Trades"=0, "Winners"=0, 
     "Losers"=0, "Longs"=0, "Shorts"=0, 
     "NetProfitLong"=0, "NetProfitShort"=0, 
     "GrossProfit"=0, "GrossLoss"=0, 
     "LargestWinner"=0, "LargestLoser"=0, 
     "LargestWinningStreak"=0, "LargestLosingStreak"=0, 
     "TotalNetProfit"=0, "TradeXML"='', "WinLossRatio"=0, 
     "AveWin"=0, "AveLoss"=0, "AveTradeDuration"=0, 
     "AveWinDuration"=0, "AveLossDuration"=0, "StdDevAllTrades"=0, 
     "StdDevWinTrades"=0, "StdDevLossTrades"=0, 
     "SharpRatio"=0, "SortinoRatio"=0, 
     "PNL_Last3"=0, "PNL_Last5"=0, "PNL_Last8"=0, "PNL_Last13"=0, 
     "PNL_Last21"=0, "PNL_Last34"=0 
	WHERE "GroupID" < 51;


    Delete from "public"."LiveOrder";
    delete from "public"."ClosedTrades";
    delete from public."ModelOrderLog";

    delete from "public"."ScoreCard" where "GroupID" < 50;
    delete from "public"."UserProfiles" where "GroupID" < 50;
