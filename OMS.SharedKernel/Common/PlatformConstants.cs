namespace OMS.SharedKernel.Common;

public class PlatformConstants
{
    // ORDER Constants
    public const string INVALID_SYMBOL = "InvalidSymbol";

    /*
    // Kafka Constants
    public const string KAFKA_BOOTSTRAP_SERVERS = "10.0.0.50:9092";
    public const string MODEL_ORDER_TOPIC_NAME = "model-orders";
    public const string STRATEGY_ORDER_TOPIC_NAME = "strategy-orders";
    public const string MODEL_ORDER_LOG_TOPIC_NAME = "model-order-log";

    // Pulsar Constants
    public const string PULSAR_STRATEGY_ORDER_TOPIC_PART =  "persistent://public/strategy/";
    public const string PULSAR_MODEL_ORDER_TOPIC_PART =  "persistent://public/models/";
    public const string PULSAR_MODEL_ORDER_LOG_TOPIC_PART =  "persistent://public/models/";
    public const string PULSAR_URI = "pulsar://10.0.0.50:6650";

    */
    
    // Postgres Constants
    //public const string LOCAL_CONN ="Host=localhost;Database=orders;Username=trading;Password=abc";
    //public const string REMOTE_CONN ="Host=10.0.0.50;Database=timedata;Username=omsuser;Password=abc";
    //public const string CYBER_CONN ="Host=10.0.0.240;Database=omsuser;Username=omsuser;Password=abc";
    //public const string CURRENT_CONN = LOCAL_CONN;

    // Order API Constants
	//public const string OSX_BASE_URL = "http://10.0.0.147:8786/";
	//public const string CYBER_BASE_URL = "http://10.0.0.240:8786/";
	
    //CHANGE BASED ON ENVIRONMENT
	//public const string OM_BASE_URL = OSX_BASE_URL;
	
    public const string ML_ORDER_URI="order/ml/process-order";
    public const string ML_ORDER_URI_Z="order/ml/process-order-z";
    public const string STRATEGY_ORDER_URI="order/strategy/process-order";

    // User API Constants
    public const string ADD_TRADER_URI="user/add-new-strategy-trader";
    public const string VERIFY_AND_ADD_BY_DISPLAY_NAME_URI="user/verify-add-by-displayName";
    public const string VERIFY_MODEL_TRADER_URI="user/verify-model-trader";

    // Data Analysis API Constants
    public const string GET_LIVE_ORDERS_BY_TRADER="data/orders/live/{profileKey}";
    public const string GET_LIVE_ORDERS_BY_TRADER_PART="data/orders/live/";

    public const string GET_LAST_CLOSED_TRADE_BY_OPEN_PLATFORM_ID="data/trades/closed/last/{profileKey}/{platformId}";
    public const string GET_LAST_CLOSED_TRADE_BY_OPEN_PLATFORM_ID_PART="data/trades/closed/last/";
    public const string GET_SCORE_CARD="data/scoreCard/{profileKey}";


    // Strategy Runner API Constants
	//public const string OSX_STRATEGY_RUNNER_BASE_URL = "http://10.0.0.147:9999/";
	//public const string CYBER_STRATEGY_RUNNER_BASE_URL = "http://10.0.0.240:9999/";
    //public const string STRATEGY_RUNNER_BASE_URL = OSX_STRATEGY_RUNNER_BASE_URL;
	
    public const string STRATEGY_RUNNER_EVALUATE="api/strategy/evaluate";

    // Feature Data API Constants
    public const string PROCESS_FEATURE_DATA_PREDICT="data/new-feature-data";
    public const string PROCESS_RAW_FEATURE_DATA="data/raw-feature-data";

    //public const string NT_ORDER_SERVICE_BASE_URL = "http://10.0.0.147:9898/";
    public const string NT_SIGNALS_PREDICT = "predict";

    //public const string QUEST_DB_CONN = "http::addr=10.0.0.50:9999;";

}
