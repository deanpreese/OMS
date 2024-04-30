using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.SharedKernel.Common;


public class CommonApiClient : ScreenColorBase
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

    public CommonApiClient(string baseUrl = PlatformConstants.OM_BASE_URL)
    {
        BaseUrl = baseUrl;
    }

    //endpoints.MapPost("api/strategyapi/verify-add-by-displayName", async (NewTraderDTO newTrader, IUserService userService) =>
    public async Task<int> VerifyAndAddByDisplayNameAsync(NewTraderDTO newTrader) =>
        await PostAsync<int, NewTraderDTO>(PlatformConstants.VERIFY_AND_ADD_BY_DISPLAY_NAME_URI, newTrader);


    //endpoints.MapPost("api/strategyapi/process-order", async (NewOrderDTO order, ITradingService tradingService) =>
    public async Task<int> ProcessOrderAsync(NewOrderDTO order) =>
        await PostAsync<int, NewOrderDTO>(PlatformConstants.STRATEGY_ORDER_URI, order);


    //endpoints.MapGet("api/strategyapi/orders/live/{profileKey}", async (string profileKey, IDataService dataService ) =>
    public async Task<List<LiveOrderDTO>> GetLiveOrdersAsync(string profileKey) 
    {
        //var orders =  await GetAsync<List<LiveOrderDTO>>($"api/strategyapi/orders/live/{profileKey}");
        var orders =  await GetAsync<List<LiveOrderDTO>>($"{PlatformConstants.GET_LIVE_ORDERS_BY_TRADER_PART}{profileKey}");
        return orders;
    }

    //endpoints.MapGet("api/strategyapi/trades/closed/last/{profileKey}/{platformId}"
    public async Task<ClosedTradeDTO> GetLastClosedTradeByOpenPlatformIDAsync(string profileKey, int platformId) 
    {
        var closedTrade =  await GetAsync<ClosedTradeDTO>($"{PlatformConstants.GET_LAST_CLOSED_TRADE_BY_OPEN_PLATFORM_ID_PART}{profileKey}/{platformId}");    
        return closedTrade;
    }
        

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
