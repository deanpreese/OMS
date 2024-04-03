using System.Text.Json;
using System.Text.Json.Serialization;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Strategy.SharedKernel;

public class ModelOrderLogDTO
{
    static ScreenColorBase scb = new ScreenColorBase();

    static JsonSerializerOptions options = new JsonSerializerOptions {
            //NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters ={
                new JsonStringEnumConverter(),
                new CustomDoubleConverter()
            },
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true
        };


    public string _ct { get; set; }
    public int ModelOrderLogID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public int LiveOrderIDReference { get; set; }
    public int OrderType { get; set; }
    public int OrderAction { get; set; }
        
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

    //[JsonIgnore]
    public ScoreCardDTO ScoreCardDeserialized()
    {
     
        ScoreCardDTO sc = new ScoreCardDTO();

        try {

            sc =JsonSerializer.Deserialize<ScoreCardDTO>(ScoreCardJson, options);

            //Console.WriteLine($"{scb.GREEN} SC  {sc.SharpRatio}  {sc.SortinoRatio}");
            //Console.WriteLine(ScoreCardJson);
            //Console.ResetColor();



        }catch (Exception ex) {
            Console.WriteLine($"{scb.YELLOW}{ex}");
            Console.WriteLine("--------------");
            Console.WriteLine(ScoreCardJson);
            Console.WriteLine("--------------");
            Console.ResetColor();
        }

        return sc;
        
    } 

    [JsonPropertyName("ClosedOrderDTOJSON")]
    public string ClosedOrderJson { get; set; }
    [JsonIgnore]
    public ClosedTradeDTO ClosedTradeDeserialized => JsonSerializer.Deserialize<ClosedTradeDTO>(ClosedOrderJson);

}


