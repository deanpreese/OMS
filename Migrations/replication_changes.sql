ALTER SYSTEM SET wal_level='logical';
ALTER SYSTEM SET max_wal_senders='10';
ALTER SYSTEM SET max_replication_slots='10';

CREATE PUBLICATION liveupdates
    FOR TABLE ONLY public."LiveOrder"
    WITH (publish = 'insert, update, delete, truncate', publish_via_partition_root = false);