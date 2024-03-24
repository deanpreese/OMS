SELECT
    t1."UserID" as "Trader",
    t1."GroupID" as "TraderGroup",
    t2."UserID" as "Strategy",
    t2."GroupID" as "StrategyGroup",
    t1."OpenPlatformOrderID" as "TraderPOID",
    t2."OpenRelatedOrderID" as "TraderORID",
    t1."OpenOrderPX" as "OpenPX",
    m1."DateCreated" ,
    m1."ModelFeatureData" ,
    m1."ScoreCardJSON"
    FROM "ClosedTrades" t1 
    JOIN "ClosedTrades" t2 ON t1."OpenPlatformOrderID" = t2."OpenRelatedOrderID"
    JOIN "ModelOrderLog" m1 on t1."OpenPlatformOrderID" = m1."PlatformOrderID"
    ;

