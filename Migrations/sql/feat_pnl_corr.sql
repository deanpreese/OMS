--SELECT * FROM windowed_correlation(40, 100, 'rsi', 'PNL');

CREATE OR REPLACE FUNCTION feat_pnl_corr(
    group_id INT,
    window_size INT,
    column_a_name TEXT
)
RETURNS TABLE (
    featuretime TIMESTAMP WITH TIME ZONE,
	pnl  FLOAT,
	corr_result FLOAT
) AS $$
DECLARE
    sql_query TEXT;
BEGIN
    sql_query := format('
        SELECT
            fd.featuretime,
			ct."PNL" as pnl,
            corr(fd.%I, ct."PNL") OVER (
                ORDER BY fd.featuretime
                ROWS BETWEEN %s PRECEDING AND CURRENT ROW
            ) AS corr_result
        FROM
            public."ClosedTrades" ct
        JOIN
            public.full_feature_data fd ON ct."OpenOrderTime" = fd.featuretime
        WHERE
            ct."GroupID" = %s
        ',
        column_a_name, window_size, group_id
    );

    RETURN QUERY EXECUTE sql_query;
END;
$$ LANGUAGE plpgsql;
