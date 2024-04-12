using System.Text;
using Newtonsoft.Json;

using OMS.SharedKernel.DTO;

namespace Strategy.Ninja;

public static class OMSClient
{
    // =============================================================
    // ML API
    // =============================================================
    //endpoints.MapPost("api/mlorders/process-order", async (NewOrderDTO order, NewOrderChannelService newOrderChannelService) =>
    public static async Task<int> ProcessMLOrderAsync(NewOrderDTO newOrder, string server_url = "http://localhost:8786")
    {
        using (HttpClient client = new HttpClient())
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(newOrder), Encoding.UTF8, "application/json");
            var response = client.PostAsync(server_url + "/api/mlorders/process-order", jsonContent);

            if (!response.IsCompletedSuccessfully)
            {
                Console.WriteLine($"ProcessOrder ML Result: {response.Result}");
                return await Task.FromResult(0); // Parse the result string to an integer before returning
            }
            else
            {
                Console.WriteLine($"ProcessOrder ML Failed. Status Code: {response.Exception}");
                var result = response.Result.Content.ReadAsStringAsync().Result;
                return Int32.Parse(result); // Add return statement
            }
        }
    }
    //endpoints.MapPost("api/mlorders/verify-model-trader", async (NewTraderDTO newTrader, IUserService userService) =>
    public static async Task<int> VerifyModelTrader(NewTraderDTO newTrader, string server_url = "http://localhost:8786")
    {
            using (HttpClient client = new HttpClient())
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(newTrader), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(server_url + "/api/mlorders/verify-model-trader", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Auth Result: {result}");
                    return await Task.FromResult(0);
                }
                else
                {
                    Console.WriteLine($"Auth Failed. Status Code: {response.StatusCode}");
                    return 0;
                }
                
            }
    }


    // =============================================================
    // Strategy API
    // =============================================================

     //endpoints.MapPost("api/strategyapi/add-new-strategy-trader", async (NewTraderDTO newTrader, IUserService userService) => 
    public static async Task<int> AddStrategyTraderAsync(NewTraderDTO newTrader, string server_url = "http://localhost:8786")
    {
            using (HttpClient client = new HttpClient())
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(newTrader), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(server_url + "/api/strategyapi/add-new-strategy-trader", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Add Result: {result}");
                    return await Task.FromResult(0);
                }
                else
                {
                    Console.WriteLine($"Add Failed. Status Code: {response.StatusCode}");
                    return 0;
                }
            }
    }

   
    //endpoints.MapPost("api/strategyapi/auth-by-displayName", async (NewTraderDTO newTrader, IUserService userService) =>
    public static async Task<int> StrategyAuthByDisplayName(NewTraderDTO newTrader, string server_url = "http://localhost:8786")
    {
        using (HttpClient client = new HttpClient())
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(newTrader), Encoding.UTF8, "application/json");
            var response = client.PostAsync(server_url + "/api/strategyapi/auth-by-displayName", jsonContent);


            if (!response.IsCompletedSuccessfully)
            {
                Console.WriteLine($"ProcessOrder ML Result: {response.Result}");
                return await Task.FromResult(0);
            }
            else
            {
                Console.WriteLine($"ProcessOrder ML Failed. Status Code: {response.Exception}");
                var result = response.Result.Content.ReadAsStringAsync().Result;
                return Int32.Parse(result); // Add return statement
            }
        }        
    }

    
}
