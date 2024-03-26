using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using OMS.SharedKernel.DTO;

namespace OMS.SharedKernel;

public class StrategyApiClient
{
    JsonSerializerOptions options = new JsonSerializerOptions {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
                Converters ={
                    new JsonStringEnumConverter()
                }
    };


    private readonly HttpClient _httpClient;
    private string _baseUrl = "http://10.0.0.147:8786";

    public StrategyApiClient(HttpClient httpClient, string baseUrl = "http://localhost:8786")
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }


    //endpoints.MapPost("api/strategyapi/add-new-strategy-trader", async (NewTraderDTO newTrader, IUserService userService) => 
    public async Task<int> AddNewTraderAsync(NewTraderDTO newTrader) =>
        await PostAsync<int, NewTraderDTO>("/api/strategyapi/add-new-strategy-trader", newTrader);


    //endpoints.MapPost("api/strategyapi/verify-add-by-displayName", async (NewTraderDTO newTrader, IUserService userService) =>
    public async Task<int> VerifyAndAddByDisplayNameAsync(NewTraderDTO newTrader) =>
        await PostAsync<int, NewTraderDTO>("/api/strategyapi/verify-add-by-displayName", newTrader);


    //endpoints.MapPost("api/strategyapi/add-new-scoreCard", async (NewTraderDTO newTrader, IAnalyticsService analyticsService) =>
    public async Task<int> AddScoreCardForTraderAsync(NewTraderDTO newTrader) =>
        await PostAsync<int, NewTraderDTO>("/api/strategyapi/add-new-scoreCard", newTrader);


    //endpoints.MapPost("api/strategyapi/process-order", async (NewOrderDTO order, ITradingService tradingService) =>
    public async Task<int> ProcessOrderAsync(NewOrderDTO order) =>
        await PostAsync<int, NewOrderDTO>("/api/strategyapi/process-order", order);


    //endpoints.MapGet("api/strategyapi/orders/live/{profileKey}", async (string profileKey, IDataService dataService ) =>
    public async Task<List<LiveOrderDTO>> GetLiveOrdersAsync(string profileKey) =>
        await GetAsync<List<LiveOrderDTO>>($"/api/strategyapi/orders/live/{profileKey}");


    //endpoints.MapGet("api/strategyapi/trades/closed/last/{profileKey}/{platformId}", async (string profileKey, int platformId, IDataService dataService) =>
    public async Task<ClosedTradeDTO> GetLastClosedTradeByOpenPlatformIDAsync(string profileKey, int platformId) =>
        await GetAsync<ClosedTradeDTO>($"/api/strategyapi/trades/closed/last/{profileKey}/{platformId}");


    //endpoints.MapGet("api/strategyapi/scoreCard/{profileKey}", async (string profileKey, IAnalyticsService analyticsService) =>
    public async Task<ScoreCardDTO> GetScoreCardAsync(string profileKey) =>
        await GetAsync<ScoreCardDTO>($"/api/strategyapi/scoreCard/{profileKey}");



    private async Task<TResponse> PostAsync<TResponse, TRequest>(string uri, TRequest content)
    {
        var response = await _httpClient.PostAsJsonAsync(_baseUrl + uri, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(options);
    }

    private async Task<T> GetAsync<T>(string uri)
    {
        var response = await _httpClient.GetAsync(_baseUrl + uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(options);
    }


}
