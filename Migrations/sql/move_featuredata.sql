INSERT INTO 
	public."FeatureData" 
	("FeatureSetID", "FeatureSetData", "FeatureSetName", 
	"FeatureNameData", "TimeTicks", "Instrument", "Open", 
	"High", "Low", "Close")
SELECT "FeatureSetID", "FeatureSetData", "FeatureSetName", "FeatureNameData", "TimeTicks", "Instrument", "Open", "High", "Low", "Close"
FROM dblink('dbname=orders host=10.0.0.147 user=dean password=abc',
            'SELECT "FeatureSetID", "FeatureSetData", 
			"FeatureSetName", "FeatureNameData", "TimeTicks", 
			"Instrument", "Open", "High", "Low", "Close"
             FROM public."FeatureData"')
AS t("FeatureSetID" integer, "FeatureSetData" text, 
	"FeatureSetName" text, "FeatureNameData" text, 
	"TimeTicks" bigint, "Instrument" text, "Open" double precision, 
	"High" double precision, "Low" double precision, "Close" double precision);
