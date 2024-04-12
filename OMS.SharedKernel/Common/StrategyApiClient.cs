using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.SharedKernel;

public class StrategyApiClient : ScreenColorBase
{

    JsonSerializerOptions options = new JsonSerializerOptions {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters ={
                new JsonStringEnumConverter()
            },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true
  };


    public string BaseUrl {get;set;}

    public StrategyApiClient(string baseUrl = "http://localhost:8786")
    {
        //_httpClient = new HttpClient();
        BaseUrl = baseUrl;
    }

    public async Task<string> SendCsvDataAsync(string url, string csvData)
    {
        var _httpClient = new HttpClient();
        try
        {
            // Assuming the server expects the content type to be 'text/csv'
            var content = new StringContent(csvData, Encoding.UTF8, "text/csv");

            // Perform the POST request
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Read the response body
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


    //endpoints.MapPost("api/strategyapi/add-new-strategy-trader", async (NewTraderDTO newTrader, IUserService userService) => 
    public async Task<int> AddNewTraderAsync(NewTraderDTO newTrader) =>
        await PostAsync<int, NewTraderDTO>("/api/strategyapi/add-new-strategy-trader", newTrader);


    //endpoints.MapPost("api/strategyapi/verify-add-by-displayName", async (NewTraderDTO newTrader, IUserService userService) =>
    public async Task<int> VerifyAndAddByDisplayNameAsync(NewTraderDTO newTrader) =>
        await PostAsync<int, NewTraderDTO>("/api/strategyapi/verify-add-by-displayName", newTrader);


    //endpoints.MapPost("api/strategyapi/process-order", async (NewOrderDTO order, ITradingService tradingService) =>
    public async Task<int> ProcessOrderAsync(NewOrderDTO order) =>
        await PostAsync<int, NewOrderDTO>("/api/strategyapi/process-order", order);


    //endpoints.MapGet("api/strategyapi/orders/live/{profileKey}", async (string profileKey, IDataService dataService ) =>
    public async Task<List<LiveOrderDTO>> GetLiveOrdersAsync(string profileKey) 
    {
        var orders =  await GetAsync<List<LiveOrderDTO>>($"/api/strategyapi/orders/live/{profileKey}");
        return orders;
    }

    //endpoints.MapGet("api/strategyapi/trades/closed/last/{profileKey}/{platformId}", async (string profileKey, int platformId, IDataService dataService) =>
    public async Task<ClosedTradeDTO> GetLastClosedTradeByOpenPlatformIDAsync(string profileKey, int platformId) =>
        await GetAsync<ClosedTradeDTO>($"/api/strategyapi/trades/closed/last/{profileKey}/{platformId}");


    //endpoints.MapGet("api/strategyapi/scoreCard/{profileKey}", async (string profileKey, IAnalyticsService analyticsService) =>
    public async Task<ScoreCardDTO> GetScoreCardAsync(string profileKey) =>
        await GetAsync<ScoreCardDTO>($"/api/strategyapi/scoreCard/{profileKey}");



    private async Task<TResponse> PostAsync<TResponse, TRequest>(string uri, TRequest content)
    {
        var _httpClient = new HttpClient();
        var response = await _httpClient.PostAsJsonAsync(BaseUrl + uri, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(options);
    }

    private async Task<T> GetAsync<T>(string uri)
    {
        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(BaseUrl + uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(options);
    }


}
