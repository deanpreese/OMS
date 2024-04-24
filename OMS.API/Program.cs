
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OMS.Application.Interfaces;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Interfaces;
using OMS.Infrastructure.Queue;
using OMS.Infrastructure.Services;
using OMS.Infrastructure.Services.Common;
using OMS.SharedKernel.Common;

using OMS.API;
using System.Text.Json;
using OMS.SharedKernel;

var builder = WebApplication.CreateBuilder(args);
string conn =  PlatformConstants.conn_in_use;

// Add services to the container.
builder.Services.AddDbContext<OrderManagementDbContext>(options =>
{   
    options.UseNpgsql(conn);
    options.EnableThreadSafetyChecks();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApiDocument();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILiveOrderRepository, LiveOrderRepository>();
builder.Services.AddScoped<IClosedOrderRepository, ClosedOrderRepository>();
builder.Services.AddScoped<ITradingService, TradingService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddSingleton<IPlatformOrderIDGen, PlatformOrderIDGen>();      
builder.Services.AddSingleton<OrderManagerService>();  

builder.Services.AddSingleton<NewOrderChannelService>();
builder.Services.AddSingleton<ClosedOrderChannelService>();
builder.Services.AddHostedService<NewOrderProcessorService>();
builder.Services.AddHostedService<ClosedOrderProcessorService>();
//builder.Services.AddHostedService<PulsarNewOrderWatcherService>();

builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddHttpsRedirection(opt => opt.HttpsPort = 44300);


builder.Services.Configure<JsonOptions>( options =>  
{
     options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
     options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
}
);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseOpenApi();
app.MapMLOrdersEndpoints();
app.MapStrategyOrdersEndpoints();


app.Run();
