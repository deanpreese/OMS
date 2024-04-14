	SELECT exp.name as experiment_name, *
	
    
    FROM crosstab('SELECT run_uuid, metrics.key, value FROM public.metrics ORDER BY 1,2',
                             'SELECT DISTINCT metrics.key FROM public.metrics ORDER BY 1')
							 
            AS ct(run_uuid TEXT, 
				"Ave Dwn Miss" DOUBLE PRECISION, --R 
				"Ave Loss Miss" DOUBLE PRECISION, --R
				"Ave Loss Streak" DOUBLE PRECISION, --R
				"Ave Up Miss" DOUBLE PRECISION, --R
				"Ave Win Miss" DOUBLE PRECISION, --R
				"Ave Win Streak" DOUBLE PRECISION, --R
				"correctP" INTEGER, --O
				"correctX" INTEGER, --O
				"correctY" INTEGER, --O
				"cpp" DOUBLE PRECISION, --O 
				"cxp" DOUBLE PRECISION, --O
				"cyp" DOUBLE PRECISION, --O
				"FeatureCount" INTEGER, --O
				"Longest Loss Streak" INTEGER, --R
				"Longest Win Streak" INTEGER,  --R
				"MAE" DOUBLE PRECISION, --R
				"MSE" DOUBLE PRECISION, --R
				"Perf" DOUBLE PRECISION, --R
				"R2" DOUBLE PRECISION, --R
				"RMSE" DOUBLE PRECISION, --R
				"Score" DOUBLE PRECISION, --R
				"Total" INTEGER, --R
				"totalX" INTEGER --O
				 ) 

    
	join public.runs as r on r.run_uuid = ct.run_uuid	
    join public.experiments as exp on r.experiment_id = exp.experiment_id 

	where
		ct.run_uuid in (
		
		select r.run_uuid 
			from runs as r 
			join public.experiments as exp on r.experiment_id = exp.experiment_id 
			join public.metrics as met on r.run_uuid = met.run_uuid
			where exp.name like '%output%' AND exp.lifecycle_stage = 'active' 
		)
		
	order by ct.cpp DESC	;