
DELETE FROM PUBLIC."ClosedTrades"  where "GroupID" < 50;
DELETE FROM PUBLIC."ScoreCardLog" where "GroupID" < 50;
DELETE FROM public."ModelOrderLog"  where "GroupID" < 50;
DELETE FROM PUBLIC."OrderLog" where "GroupID" < 50;
DELETE FROM PUBLIC."ScoreCards" where "GroupID" < 50;
DELETE FROM PUBLIC."UserProfiles" where "GroupID" < 50;

DELETE FROM PUBLIC."ActivityLogs" ;
DELETE FROM PUBLIC."LiveOrders" ;

