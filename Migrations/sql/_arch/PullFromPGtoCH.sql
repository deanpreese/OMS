--INSERT INTO `default`.ClosedTradeLogCopy
--SELECT * FROM postgresql('localhost:5432', 'orders', 'ClosedTradeLog', 'clickhouse', 'abc');

--delete from `default`.ClosedTradeLogCopy

/*
SELECT max(`StorerID`) AS maxIntID FROM `default`.ClosedTradeLogCopy;
INSERT INTO `default`.ClosedTradeLogCopy
SELECT * FROM postgresql('localhost:5432', 'orders', 'ClosedTradeLog', 'clickhouse', 'abc');
WHERE StorerID > maxIntID;
*/