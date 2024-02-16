
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

using OMS.Core.Models;
using Spectre.Console;

namespace OMS.Core.WebAPIClient;

public static class OMSClient
{

    // -------------------------------------------------------------
    public static async Task<int> AddTraderAsync(NewTrader newTrader, string server_url = "http://localhost:8786")
    {
            using (HttpClient client = new HttpClient())
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(newTrader), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(server_url + "/api/order/add-new-trader", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    AnsiConsole.WriteLine($"Add Result: {result}");
                    return int.Parse(result); 
                }
                else
                {
                    AnsiConsole.WriteLine($"Add Failed. Status Code: {response.StatusCode}");
                    return 0;
                }
            }
    }



    // -------------------------------------------------------------
    public static async Task<int> AuthenticateTraderAsync(UserInfo newTrader, string server_url = "http://localhost:8786")
    {
            using (HttpClient client = new HttpClient())
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(newTrader), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(server_url + "/api/order/authenticate", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    AnsiConsole.WriteLine($"Auth Result: {result}");
                    return int.Parse(result); 
                }
                else
                {
                    AnsiConsole.WriteLine($"Auth Failed. Status Code: {response.StatusCode}");
                    return 0;
                }
                
            }
    }


    // -------------------------------------------------------------
    public static async Task<int> SendOrderAsync(NewOrder newOrder, string server_url = "http://localhost:8786")
    {
        using (HttpClient client = new HttpClient())
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(newOrder), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(server_url + "/api/order/process-order", jsonContent);

            var result = await response.Content.ReadAsStringAsync();
            int code = int.Parse(result);

            return code;

        }
    }


    // -------------------------------------------------------------
    public static int SendMLOrderAsync(NewOrder newOrder, string server_url = "http://localhost:8786")
    {
        using (HttpClient client = new HttpClient())
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(newOrder), Encoding.UTF8, "application/json");
            var response = client.PostAsync(server_url + "/api/order/process-ml-order", jsonContent);


            if (!response.IsCompletedSuccessfully)
            {
                AnsiConsole.WriteLine($"ProcessOrder ML Result: {response.Result}");
                return 0; // Parse the result string to an integer before returning
            }
            else
            {
                AnsiConsole.WriteLine($"ProcessOrder ML Failed. Status Code: {response.Exception}");
                var result = response.Result.Content.ReadAsStringAsync().Result;
                return Int32.Parse(result); // Add return statement
            }
        }
    }



    // -------------------------------------------------------------
    public static async Task<List<UserProfile>>  GetTraders(int numberOfTraders, string server_url = "http://localhost:8786")
    {
        List<UserProfile> return_list = new List<UserProfile>();

        using (HttpClient client = new HttpClient())
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(numberOfTraders), Encoding.UTF8, "application/json");
            var response = client.PostAsync(server_url + "/api/data/get-traders", jsonContent);
            

            if (!response.IsCompletedSuccessfully)
            {
                AnsiConsole.WriteLine($"ProcessOrder ML Result: {response.Result}");
                var result = await response.Result.Content.ReadAsStringAsync();
                var deserializedResult = JsonConvert.DeserializeObject<List<UserProfile>>(result);
                return_list = deserializedResult ?? new List<UserProfile>();
            }
            else
            {
                AnsiConsole.WriteLine($"ProcessOrder ML Failed. Status Code: {response.Exception}");
                var result = await response.Result.Content.ReadAsStringAsync();
                return_list = new List<UserProfile>();
            }
            
        }
       return return_list;
    }


}
