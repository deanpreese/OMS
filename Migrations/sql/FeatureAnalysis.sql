select 
	fd.featuretime,
    SUM(ct."PNL") OVER (ORDER BY featuretime ) AS cumulative_amount,
	corr(fd.rsi, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as rsi_corr_100,

	corr(fd.stok1, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as stok1_corr_100,
	
	corr(fd.atr2, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as atr2_corr_100,

	corr(fd.atr21, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as atr21_corr_100,
	
	corr(fd.atr3, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as atr3_corr_100,

	corr(fd.atr31, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as atr31_corr_100,

	corr(fd.atr32, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as atr32_corr_100,

	corr(fd.atr34, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as atr34_corr_100,

	corr(fd.roc, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as roc_corr_100,

	corr(fd.sdkc9, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
	
             ) as sdkc9_corr_100,
	
	corr(fd.scdk91, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as scdk91_corr_100,
	
	corr(fd.sdbb91, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as sdbb91_corr_100,
	
	corr(fd.sdlr310, ct."PNL") 
		OVER (
	 		ORDER BY fd.featuretime 
	 		ROWS BETWEEN 100 PRECEDING AND CURRENT ROW
             ) as sdlr310_corr_100

from public."ClosedTrades" ct
join public.full_feature_data as fd on ct."OpenOrderTime"  = fd.featuretime
where ct."GroupID" <100
order by fd.featuretime desc
limit(500)

