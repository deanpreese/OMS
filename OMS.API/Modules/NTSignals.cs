using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OMS.Application.Models;
using OMS.Infrastructure.Services;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.API;

public static class NTSignals
{
    public static IEndpointRouteBuilder MapNTSignalsEndpoints(this IEndpointRouteBuilder endpoints)
    {
  
        endpoints.MapPost(PlatformConstants.PROCESS_FEATURE_DATA_PREDICT, async (IConfiguration config, OrderManagerService orderManagerService, FeatureDataDTO featureData ) =>
        {
            DateTime time = new DateTime(featureData.TimeTicks);
            string iso8601String = time.ToString("o");
            string csv_data = iso8601String + "," + featureData.FeatureSetData;

            string strategy_runner_evaluate = config.GetValue<string>("ServiceUrls:STRATEGY_RUNNER_BASE_URL") + PlatformConstants.STRATEGY_RUNNER_EVALUATE;
            string nt_url = config.GetValue<string>("ServiceUrls:NT_ORDER_SERVICE_BASE_URL") + PlatformConstants.NT_SIGNALS_PREDICT;

            List<NewOrderDTO> rtn_dto  =  await Send_ML_Data_Async(csv_data, nt_url);    

            foreach (var item in rtn_dto)
            {
                string feature_data = JsonSerializer.Serialize(featureData);
                item.ModelFeatureData = feature_data ;   

                Console.WriteLine("DTO: " + item.OrderTime +  " " + item.UserID + "  " + item.OrderAction + "  " + item.OrderType + "  " +   item.OrderPX +  " " + featureData.Instrument + "  " + featureData.FeatureSetName );                        
                ModelOrderLog mol =  await orderManagerService.ProcessNewTraderOrder(item);
                
                var _httpClient = new HttpClient();
                var response = await _httpClient.PostAsJsonAsync(strategy_runner_evaluate, mol);
                response.EnsureSuccessStatusCode();

                var dto = await response.Content.ReadFromJsonAsync<List<NewOrderDTO>>();
                

            }

            await Task.FromResult(0);

        })
        .WithName("ProcessFeatureData")
        .WithOpenApi();

        return endpoints;   
    }   


    private static async Task<List<NewOrderDTO>> Send_ML_Data_Async(string csvData, string url )
    {
        List<NewOrderDTO> ml_dto = new List<NewOrderDTO>();
        var _httpClient = new HttpClient();
        try
        {
            var content = new StringContent(csvData, Encoding.UTF8, "text/csv");
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var ml_dto_x = JsonSerializer.Deserialize<List<NewOrderDTO>>(responseBody);
            if( ml_dto_x != null )
                ml_dto = ml_dto_x;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
        return ml_dto;
    }


}
