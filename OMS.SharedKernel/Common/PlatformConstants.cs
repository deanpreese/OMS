namespace OMS.SharedKernel.Common;

public class PlatformConstants
{
    // Pulsar Constants
    public const string InvalidSymbol = "InvalidSymbol";
    public const string PULSAR_STRATEGY_ORDER_TOPIC =  "persistent://public/strategy/strategy-orders";
    public const string PULSAR_NEW_ORDER_TOPIC =  "persistent://public/models/new-model-orders";
    public const string PULSAR_MODEL_ORDER_LOG_TOPIC =  "persistent://public/models/raw-model-orders";
    //public const string pulsar_uri_string = "pulsar://10.0.0.82:6650";
    public const string pulsar_uri_string = "pulsar://10.0.0.50:6650";


    // Postgres Constants
    public const string local_conn ="Host=localhost;Database=orders;Username=trading;Password=abc";
    public const string remote_conn ="Host=10.0.0.50;Database=timedata;Username=omsuser;Password=abc";

    public const string conn_in_use = local_conn;

}
