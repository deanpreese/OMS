SELECT * FROM get_pnl_correlation_stats(140, 250, 'rsi');

/*
CREATE OR REPLACE FUNCTION get_pnl_correlation_stats(
    threshold INT,
    window_size INT,
    indicator TEXT
)
RETURNS TABLE(ind TEXT, plus_plus BIGINT, min_min BIGINT, min_plus BIGINT, plus_min BIGINT) AS $$
WITH plusplus AS (
    SELECT 
        count(pnl) as pp
    FROM feat_pnl_corr(threshold, window_size, indicator)
    WHERE pnl > 0 AND corr_result > 0
),
minmin AS (
    SELECT 
        count(pnl) as mm
    FROM feat_pnl_corr(threshold, window_size, indicator)
    WHERE pnl < 0 AND corr_result < 0
),
minplus AS (
    SELECT 
        count(pnl) as mp
    FROM feat_pnl_corr(threshold, window_size, indicator)
    WHERE pnl < 0 AND corr_result > 0
),
plusmin AS (
    SELECT 
        count(pnl) as pm
    FROM feat_pnl_corr(threshold, window_size, indicator)
    WHERE pnl > 0 AND corr_result < 0
)
SELECT 
    indicator as ind,
    pp.pp as plus_plus,
    mm.mm as min_min,
    mp.mp as min_plus,
    pm.pm as plus_min
FROM 
    plusplus pp,
    minmin mm,
    minplus mp,
    plusmin pm;
$$ LANGUAGE sql STABLE;

*/