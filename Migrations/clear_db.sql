
DELETE FROM PUBLIC."ActivityLogs" ;
ALTER SEQUENCE public."ActivityLogs_ActivityID_seq" RESTART WITH 1;

DELETE FROM PUBLIC."ClosedTrades" ; 
ALTER SEQUENCE public."ClosedTrades_StorerID_seq" RESTART WITH 1;

DELETE FROM PUBLIC."LiveOrder" ;
ALTER SEQUENCE public."LiveOrder_LiveOrderID_seq" RESTART WITH 1;

DELETE FROM PUBLIC."OrderFlow" ;
DELETE FROM public."ModelOrderLog";

DELETE FROM PUBLIC."ScoreCard" ;
ALTER SEQUENCE public."ScoreCard_ScoreCardID_seq" RESTART WITH 1;
DELETE FROM PUBLIC."UserProfiles" ;
ALTER SEQUENCE public."UserProfiles_UserID_seq" RESTART WITH 1;

