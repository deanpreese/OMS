
SELECT 
    corr("ROC"::numeric, "PNL") as corr_ROC_PNL,
    corr("RSI"::numeric, "PNL") as corr_RSI_PNL,
    corr("ATR2"::numeric, "PNL") as corr_ATR2_PNL,
    corr("ATR3"::numeric, "PNL") as corr_ATR3_PNL,
    corr("ATR21"::numeric, "PNL") as corr_ATR21_PNL,
    corr("ATR31"::numeric, "PNL") as corr_ATR31_PNL,
    corr("ATR32"::numeric, "PNL") as corr_ATR32_PNL,
    corr("ATR34"::numeric, "PNL") as corr_ATR34_PNL,
    corr("SDKC9"::numeric, "PNL") as corr_SDKC9_PNL,
    corr("STOK1"::numeric, "PNL") as corr_STOK1_PNL,
    corr("SDBB91"::numeric, "PNL") as corr_SDBB91_PNL,
    corr("SDKC91"::numeric, "PNL") as corr_SDKC91_PNL,
    corr("SDLR310"::numeric, "PNL") as corr_SDLR310_PNL
FROM  (
    SELECT 
        "PNL",
		"StorerID",	
        "OpenFeatureData" ->> 'ROC' as "ROC",
        "OpenFeatureData" ->> 'RSI' as "RSI",
        "OpenFeatureData" ->> 'ATR2' as "ATR2",
        "OpenFeatureData" ->> 'ATR3' as "ATR3",
        "OpenFeatureData" ->> 'ATR21' as "ATR21",
        "OpenFeatureData" ->> 'ATR31' as "ATR31",
        "OpenFeatureData" ->> 'ATR32' as "ATR32",
        "OpenFeatureData" ->> 'ATR34' as "ATR34",
        "OpenFeatureData" ->> 'SDKC9' as "SDKC9",
        "OpenFeatureData" ->> 'STOK1' as "STOK1",
        "OpenFeatureData" ->> 'SDBB91' as "SDBB91",
        "OpenFeatureData" ->> 'SDKC91' as "SDKC91",
        "OpenFeatureData" ->> 'SDLR310' as "SDLR310"
    FROM public."ClosedTradeLog"
	where "StorerID" <= 20888 
) as features;