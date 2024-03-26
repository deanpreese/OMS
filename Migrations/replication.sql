select * from pg_publication;

SELECT * FROM pg_replication_slots;

/*
CREATE PUBLICATION liveupdates
    FOR TABLE public."LiveOrder", public."ScoreCard", public."ClosedTrades", public."ModelOrderLog"
    WITH (publish = 'insert, update, delete, truncate', publish_via_partition_root = false);
*/

/*
SELECT * FROM pg_create_logical_replication_slot('liveorders_slot', 'wal2json');

CREATE PUBLICATION liveupdates
    FOR TABLE public."LiveOrder", public."ScoreCard", public."ClosedTrades", public."ModelOrderLog"
    WITH (publish = 'insert, update, delete, truncate', publish_via_partition_root = false);

*/




/*
select pg_terminate_backend(60967; 

SELECT pg_drop_replication_slot('pg2j_2abd5f832322444987616f646f2d744d');
*/
/*
SELECT pg_drop_replication_slot('pg2j_2abd5f832322444987616f646f2d744d');
*/ 