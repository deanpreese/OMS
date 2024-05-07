SELECT 
	featuretime as timestamp, 
	openpx as open, 
	highpx as high, 
	lowpx as low, 
	closepx as close, 

	avg(closepx)
       OVER (
	 		ORDER BY featuretime 
	 		ROWS BETWEEN 9 PRECEDING AND CURRENT ROW
             ) as avg9,
	avg(closepx)
       OVER (
	 		ORDER BY featuretime 
	 		ROWS BETWEEN 13 PRECEDING AND CURRENT ROW
             ) as avg13

FROM full_feature_data 
	