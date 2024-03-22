SELECT
    t1."UserID",
    t1."GroupID",
    t2."UserID",
    t2."GroupID",
    t1."OpenPlatformOrderID",
    t2."OpenRelatedOrderID",
    t1."OpenOrderPX",
    m1."DateCreated",
    m1."ModelFeatureData",
    m1."ScoreCardJSON"
    FROM "ClosedTrades" t1 
    JOIN "ClosedTrades" t2 ON t1."OpenPlatformOrderID" = t2."OpenRelatedOrderID"
    JOIN "ModelOrderLog" m1 on t1."OpenPlatformOrderID" = m1."PlatformOrderID"
    ;

