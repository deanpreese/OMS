
select
	to_timestamp((("TimeTicks" - 621355968000000000) / 10000000)) as FeatureTime,
    "Open" as openpx, 
    "High" as highpx,
    "Low"  as lowpx,
    "Close" as closepx,
	SPLIT_PART("FeatureSetData", ',', 1) ::NUMERIC as sdlr310,
	SPLIT_PART("FeatureSetData", ',', 2) ::NUMERIC as sdbb91,
	SPLIT_PART("FeatureSetData", ',', 3) ::NUMERIC as scdk91,
	SPLIT_PART("FeatureSetData", ',', 4) ::NUMERIC as sdkc9,
	SPLIT_PART("FeatureSetData", ',', 5) ::NUMERIC as roc,
	SPLIT_PART("FeatureSetData", ',', 6) ::NUMERIC as atr34,
	SPLIT_PART("FeatureSetData", ',', 7) ::NUMERIC as atr32,
	SPLIT_PART("FeatureSetData", ',', 8) ::NUMERIC as atr31,
	SPLIT_PART("FeatureSetData", ',', 9) ::NUMERIC as atr3,
	SPLIT_PART("FeatureSetData", ',', 10) ::NUMERIC as atr21,
	SPLIT_PART("FeatureSetData", ',', 11) ::NUMERIC as atr2,
	SPLIT_PART("FeatureSetData", ',', 12) ::NUMERIC as rsi,
	SPLIT_PART("FeatureSetData", ',', 13) ::NUMERIC as stok1
	
from public."FeatureData" FD
	