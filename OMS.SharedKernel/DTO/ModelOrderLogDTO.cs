using System.Text.Json;
using System.Text.Json.Serialization;

namespace OMS.SharedKernel.DTO;

public class ModelOrderLogDTO
{
    static JsonSerializerOptions options = new JsonSerializerOptions {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters ={
                new JsonStringEnumConverter()
            },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true
        };


    public string _ct { get; set; }
    public int ModelOrderLogID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public int LiveOrderIDReference { get; set; }
    public int OrderType { get; set; }
    public int OrderAction { get; set; }
    
    // JSON strings representing complex objects
    //public string LiveOrderJSON { get; set; }
    //public string ModelFeatureData { get; set; }
    //public string ScoreCardJSON { get; set; }
    //public string ClosedOrderDTOJSON    { get; set; }
    

    [JsonPropertyName("LiveOrderJSON")]
    public string LiveOrderJson { get; set; }
    [JsonIgnore]
    public LiveOrderDTO LiveOrderDeserialized => JsonSerializer.Deserialize<LiveOrderDTO>(LiveOrderJson);

    [JsonPropertyName("ModelFeatureData")]
    public string ModelFeatureDataJson { get; set; }
    [JsonIgnore]
    public Dictionary<string, double> ModelFeatureDataDeserialized => JsonSerializer.Deserialize<Dictionary<string, double>>(ModelFeatureDataJson);

    [JsonPropertyName("ScoreCardJSON")]
    public string ScoreCardJson { get; set; }
    [JsonIgnore]
    public ScoreCardDTO ScoreCardDeserialized => JsonSerializer.Deserialize<ScoreCardDTO>(ScoreCardJson);

    [JsonPropertyName("ClosedOrderDTOJSON")]
    public string ClosedOrderJson { get; set; }
    [JsonIgnore]
    public ClosedTradeDTO ClosedTradeDeserialized => JsonSerializer.Deserialize<ClosedTradeDTO>(ClosedOrderJson);

    /*
    [JsonIgnore]
    public LiveOrderDTO LiveOrderDeserialized => JsonSerializer.Deserialize<LiveOrderDTO>(LiveOrderJSON,options);
    [JsonIgnore]
    public Dictionary<string, double> ModelFeatureDataDeserialized => JsonSerializer.Deserialize<Dictionary<string, double>>(ModelFeatureData,options);
    [JsonIgnore]
    public ScoreCardDTO ScoreCardDeserialized => JsonSerializer.Deserialize<ScoreCardDTO>(ScoreCardJSON,options);
    [JsonIgnore]
    public ClosedTradeDTO ClosedTradeDeserialized => JsonSerializer.Deserialize<ClosedTradeDTO>(ClosedOrderDTOJSON,options);
   */

}


