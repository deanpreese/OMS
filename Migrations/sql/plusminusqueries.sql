

with plusplus as (
select 
	count(pnl) as pp
FROM feat_pnl_corr(140, 50, 'rsi')
where pnl > 0 AND corr_result > 0
),
minmin as(
select 
	count(pnl) as mm
FROM feat_pnl_corr(140, 50, 'rsi')
where pnl < 0 AND corr_result < 0
),
minplus as(
select 
	count(pnl) as mp
FROM feat_pnl_corr(140, 50, 'rsi')
where pnl < 0 AND corr_result > 0
),
plusmin as(
select 
	count(pnl) as pm
FROM feat_pnl_corr(140, 50, 'rsi')
where pnl > 0 AND corr_result < 0
)	
SELECT 
	'rsi',
    pp.pp,
    mm.mm,
    mp.mp,
    pm.pm
FROM 
    plusplus pp,
    minmin mm,
    minplus mp,
    plusmin pm;

