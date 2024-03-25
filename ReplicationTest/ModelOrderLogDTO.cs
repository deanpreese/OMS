using System.Text.Json;
using System.Text.Json.Serialization;

using OMS.SharedKernel;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace ReplicationTest;

public class ModelOrderLogDTO
{
    public string _ct { get; set; }
    public int ModelOrderLogID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public int LiveOrderIDReference { get; set; }
    public int OrderType { get; set; }
    public int OrderAction { get; set; }
    
    // JSON strings representing complex objects
    public string LiveOrderJSON { get; set; }
    public string ModelFeatureData { get; set; }
    public string ScoreCardJSON { get; set; }
    
    // Properties to hold deserialized data
    [JsonIgnore]
    public LiveOrderDTO LiveOrderDeserialized => JsonSerializer.Deserialize<LiveOrderDTO>(LiveOrderJSON);
    [JsonIgnore]
    public Dictionary<string, double> ModelFeatureDataDeserialized => JsonSerializer.Deserialize<Dictionary<string, double>>(ModelFeatureData);
    [JsonIgnore]
    public ScoreCardDTO ScoreCardDeserialized => JsonSerializer.Deserialize<ScoreCardDTO>(ScoreCardJSON);
}

/*
public class ModelOrderLogJSON
{

    public string _ct { get; set; }
    public int LiveOrderIDReference { get; set; }
    public int ModelOrderLogID { get; set; }
    public int OrderManagerID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public string  DateCreated { get; set; }
    public long PlatformOrderID { get; set; }
    public long RelatedOrderID { get; set; }
    public string Instrument { get; set; }
    public double OrderPX { get; set; }
    public OrderType OrderType { get; set; }
    public OrderAction OrderAction { get; set; }
    public int Quantity { get; set; }
    public int Leverage { get; set; }
    public int Opposite { get; set; }
    public string  OrderTime { get; set; }
    
    // Assuming these are JSON strings that represent complex objects
    public string ModelFeatureData { get; set; }
    public string ScoreCardJSON { get; set; }
    
    // Deserialize nested JSON strings into the following properties
    [JsonIgnore]
    public Dictionary<string, double> ModelFeatureDataDeserialized => JsonSerializer.Deserialize<Dictionary<string, double>>(ModelFeatureData);
    [JsonIgnore]
    public ScoreCardDTO ScoreCardDeserialized => JsonSerializer.Deserialize<ScoreCardDTO>(ScoreCardJSON);
}
*/


