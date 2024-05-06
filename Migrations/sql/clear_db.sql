
DELETE FROM PUBLIC."ClosedTrades"  ;
ALTER SEQUENCE public."ClosedTrades_StorerID_seq" RESTART WITH 1;

DELETE FROM PUBLIC."ClosedTradeLog";
DELETE FROM PUBLIC."ScoreCardLog";
DELETE FROM public."ModelOrderLog"  ;
DELETE FROM PUBLIC."OrderLog" ;


DELETE FROM PUBLIC."ActivityLogs" ;
ALTER SEQUENCE public."ActivityLogs_ActivityID_seq" RESTART WITH 1;

DELETE FROM PUBLIC."LiveOrders" ;
ALTER SEQUENCE public."LiveOrders_LiveOrderID_seq" RESTART WITH 1;

DELETE FROM PUBLIC."ScoreCards" ;
ALTER SEQUENCE public."ScoreCards_ScoreCardID_seq" RESTART WITH 1;

DELETE FROM PUBLIC."UserProfiles" ;
ALTER SEQUENCE public."UserProfiles_UserID_seq" RESTART WITH 1;

