
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


string conn =  PlatformConstants.CURRENT_CONN;


var builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddLogging(configure => configure.AddConsole());
//builder.Services.AddHttpsRedirection(opt => opt.HttpsPort = 44300);


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
app.MapUsersAPIEndpoints();
app.MapDataAnalysisEndpoints();

app.Run();


/*
var DataBuilder = WebApplication.CreateBuilder(args);
DataBuilder.Services.AddDbContext<OrderManagementDbContext>(options =>
{   
    options.UseNpgsql(conn);
    options.EnableThreadSafetyChecks();
});


DataBuilder.WebHost.UseUrls("http://10.0.0.147:5002");

DataBuilder.Services.AddEndpointsApiExplorer();
DataBuilder.Services.AddSwaggerGen();
DataBuilder.Services.AddOpenApiDocument();

DataBuilder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
DataBuilder.Services.AddScoped<ILiveOrderRepository, LiveOrderRepository>();
DataBuilder.Services.AddScoped<IClosedOrderRepository, ClosedOrderRepository>();
DataBuilder.Services.AddScoped<ITradingService, TradingService>();
DataBuilder.Services.AddScoped<IDataService, DataService>();
DataBuilder.Services.AddScoped<IUserService, UserService>();
DataBuilder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
DataBuilder.Services.AddSingleton<IPlatformOrderIDGen, PlatformOrderIDGen>();      
DataBuilder.Services.AddSingleton<OrderManagerService>();  
DataBuilder.Services.AddLogging(configure => configure.AddConsole());

//DataBuilder.Services.AddHttpsRedirection(opt => opt.HttpsPort = 44400);

var DataApp = DataBuilder.Build();
DataApp.UseSwagger();
DataApp.UseSwaggerUI();
DataApp.UseOpenApi();

DataApp.MapMLOrdersEndpoints();
DataApp.MapStrategyOrdersEndpoints();



await Task.WhenAny
(
    app.RunAsync(),
    DataApp.RunAsync()
);
*/