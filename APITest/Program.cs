using OMS.SharedKernel.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;

using OMS.SharedKernel;
using Orleans.Concurrency;
using OMS.SharedKernel.DTO;
using System.Security.Cryptography;


JsonSerializerOptions options = new JsonSerializerOptions {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters ={
                new JsonStringEnumConverter()
            },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true
  };


HttpClient httpClient = new HttpClient();
string baseUrl = "http://10.0.0.147:8786";

StrategyApiClient client = new StrategyApiClient( baseUrl);

/*
var data =await client.GetScoreCardAsync("390410_0");
var d_out = JsonSerializer.Serialize(data, options);
Console.WriteLine(d_out);


Console.WriteLine("--------------------------------------------------");
//4	592504	0	-1010840761	
var data2 = await client.GetLastClosedTradeByOpenPlatformIDAsync("592504_0", -1010840761);
var d_out2 = JsonSerializer.Serialize(data2, options);
Console.WriteLine(d_out2);


Console.WriteLine("--------------------------------------------------");

var data3 = await client.GetLiveOrdersAsync("734170_80");

foreach(var order in data3)
{
    Console.WriteLine(order.UserID + " " + order.OrderTime + " " + order.OrderAction + " " + order.Instrument + " " + order.OrderPX + " " + order.OrderType);
}

var d_out3 = JsonSerializer.Serialize(data3, options);
Console.WriteLine(d_out3);
*/

Console.WriteLine("--------------------------------------------------");

var newTrader = new NewTraderDTO {
    DisplayName = "TestTrader",
    Email = "a@a.com",
    UserPwd = "123456",
    GroupID = 0,
};

var trader = await client.VerifyAndAddByDisplayNameAsync(newTrader);
var d_out4 = JsonSerializer.Serialize(trader, options);
Console.WriteLine(d_out4);

Console.WriteLine("--------------------------------------------------");

newTrader.UserID = trader;

var sc = await client.AddScoreCardForTraderAsync(newTrader);
var d_out5 = JsonSerializer.Serialize(sc, options);
Console.WriteLine(d_out5);

Console.WriteLine("--------------------------------------------------");

var newOrder = new NewOrderDTO {
    OrderType = OrderType.OPEN,
    OrderAction = OrderAction.Buy,
    Instrument = "ES",
    OrderPX = 100,
    OrderTime = DateTime.Now,
    UserID = trader,
    GroupID = 0,
    Quantity = 1
};           
var order = await client.ProcessOrderAsync(newOrder);
var d_out6 = JsonSerializer.Serialize(order, options);
Console.WriteLine(d_out6);


Console.WriteLine("Press any key to Continue ...");
Console.ReadKey();


var newOrder2 = new NewOrderDTO {
    OrderType = OrderType.CLOSE,
    OrderAction = OrderAction.Sell,
    Instrument = "ES",
    OrderPX = 110,
    OrderTime = DateTime.Now,
    UserID = trader,
    GroupID = 0,
    Quantity = 1
};           
var order2 = await client.ProcessOrderAsync(newOrder2);
var d_out7 = JsonSerializer.Serialize(order2, options);
Console.WriteLine(d_out7);
