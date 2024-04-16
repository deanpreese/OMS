namespace OMS.SharedKernel.Common;

public class PlatformConstants
{
    // Order and Trader Constants
    public const string InvalidSymbol = "InvalidSymbol";
    public const string pulsar_strategy_topic =  "persistent://public/strategy/strategy-orders";
    public const string pulsar_new_order_topic =  "persistent://public/models/model-orders";
    public const string pulsar_uri_string = "pulsar://10.0.0.82:6650";
    public const string local_conn ="Host=localhost;Database=orders;Username=trading;Password=abc";
    public const string remote_conn ="Host=10.0.0.50;Database=timedata;Username=omsuser;Password=abc";

    public const string conn_in_use = local_conn;

}
