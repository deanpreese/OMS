using System.Text;
using OMS.Infrastructure.Services;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.API;

public static class FeatureData
{

    public static IEndpointRouteBuilder MapFeatureDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
  
        endpoints.MapPost(PlatformConstants.PROCESS_FEATURE_DATA, async (OrderManagerService orderManagerService, FeatureDataDTO featureData ) =>
        {
            //ModelOrderLog mol =  await orderManagerService.ProcessNewTraderOrder(order);
            
            DateTime time = new DateTime(featureData.TimeTicks);
            string iso8601String = time.ToString("o");

            string csv_data = iso8601String + "," + featureData.FeatureSetData;

            string o_s = featureData.Instrument + "  " + featureData.FeatureSetName + "  " +  new DateTime(featureData.TimeTicks) + "  " + featureData.Close + "  " + featureData.FeatureSetData;        

            if (featureData.TimeTicks < DateTime.UtcNow.Ticks - 150000000 )
            {
                await SendCsvDataAsync(csv_data, "http://10.0.147:8888/predict");    
                Console.WriteLine("Hist: " + featureData.Instrument + "  " + featureData.FeatureSetName + "  " +  new DateTime(featureData.TimeTicks) + "  UTC " + featureData.FeatureSetData);        
            }else
            {
                await SendCsvDataAsync(csv_data, "http://10.0.0.147:8888/predict");        
                Console.WriteLine("RT: " + featureData.Instrument + "  " + featureData.FeatureSetName + "   UTC " +new DateTime(featureData.TimeTicks) );    
            }


            await Task.FromResult(0);

        })
        .WithName("ProcessFeatureData")
        .WithOpenApi();

        return endpoints;   
    }   


    private static async Task<string> SendCsvDataAsync(string csvData, string url )
    {
        var _httpClient = new HttpClient();
        try
        {
            var content = new StringContent(csvData, Encoding.UTF8, "text/csv");
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
            return e.Message;
        }
    }


}
