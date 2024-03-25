
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OMS.Core.Interfaces;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Interfaces;
using OMS.Infrastructure.Queue;
using OMS.Infrastructure.Services;
using OMS.Infrastructure.Services.Common;
using OMS.SharedKernel.DTO;

var builder = WebApplication.CreateBuilder(args);

 string conn =  "Server=127.0.0.1;Database=orders;Username=trading;Password=abc";

// Add services to the container.
builder.Services.AddDbContext<OrderManagementDbContext>(options =>
{   
    options.UseNpgsql(conn);
    options.EnableThreadSafetyChecks();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILiveOrderRepository, LiveOrderRepository>();
builder.Services.AddScoped<IClosedOrderRepository, ClosedOrderRepository>();
builder.Services.AddScoped<ITradingService, TradingService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddSingleton<IPlatformOrderIDGen, PlatformOrderIDGen>();        
builder.Services.AddSingleton<NewOrderChannelService>();
builder.Services.AddSingleton<ClosedOrderChannelService>();
builder.Services.AddHostedService<NewOrderProcessorService>();
builder.Services.AddHostedService<ClosedOrderProcessorService>();

builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddHttpsRedirection(opt => opt.HttpsPort = 44300);

builder.Services.Configure<JsonOptions>( options =>  options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


// Example endpoints
app.MapPost("api/ml/process-order", async (NewOrderDTO order, NewOrderChannelService newOrderChannelService) =>
{
    int om_id = 0;
    try
    {
        await newOrderChannelService.WriteAsync(order);
    }
    catch (Exception ex)
    {
        app.Logger.LogInformation(ex.Message); 
    }
    return Results.Ok(om_id);
})
.WithName("ProcessOrder")
.WithOpenApi();


app.MapPost("api/ml/verify-model-trader", async (NewTraderDTO newTrader, IUserService userService) =>
{
    int oid = await userService.VerifyAndAddByDisplayName(newTrader);
    return Results.Ok(oid);
})
.WithName("VerifyModelTrader")
.WithOpenApi();

app.Run();
