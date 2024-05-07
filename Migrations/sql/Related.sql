With cdata as (
SELECT
	t1."UserID" as "Trader",
    t1."GroupID" as "TraderGroup",
    t2."UserID" as "Strategy",
    --t2."GroupID" as "StrategyGroup",
    --t1."OpenPlatformOrderID" as "TraderPOID",
    --t2."OpenRelatedOrderID" as "TraderORID",
    t1."PNL" as "Trader_PNL",
	t2."PNL" as "Strategy_PNL",
	
	CASE
  		--WHEN t1."PNL" < 0 THEN -1
  		WHEN t1."PNL" > 0 THEN 1
	END
	as TraderWinLoss,
	
	CASE
  		--WHEN t2."PNL" < 0 THEN -1
  		WHEN t2."PNL" > 0 THEN 1
	END
	as StratWinLoss

    FROM "ClosedTrades" t1 
    JOIN "ClosedTradeLog" t2 ON t1."OpenPlatformOrderID" = t2."OpenRelatedOrderID"
	
)


SELECT
	UP."DisplayName",
    "Trader",
    t1."TraderGroup",
    "Strategy",
    --t2."GroupID" as "StrategyGroup",
    --t1."OpenPlatformOrderID" as "TraderPOID",
    --t2."OpenRelatedOrderID" as "TraderORID",
    SUM("Trader_PNL") as "Trader_Val",
	SUM("Strategy_PNL") as "Strategy_Val",
	Count("traderwinloss") as "T_C",
	Count("stratwinloss") AS "S_C"
    FROM "cdata" t1 
	INNER JOIN 
    	public."UserProfiles" AS UP ON t1."Trader" = UP."UserID"
	Group BY "Trader", "Strategy", UP."DisplayName", "TraderGroup"
	Order BY "TraderGroup"


