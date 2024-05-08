using Microsoft.EntityFrameworkCore;
using OMS.Application.Interfaces;
using OMS.DataManager;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Interfaces;
using OMS.Infrastructure.Services;
using OMS.Infrastructure.Services.Common;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;


var builder = WebApplication.CreateBuilder(args);


// Build a config object, using env vars and JSON providers.
IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();


builder.Services.AddDbContext<OrderManagementDbContext>(options =>
{   
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    options.EnableThreadSafetyChecks();
});


builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILiveOrderRepository, LiveOrderRepository>();
builder.Services.AddScoped<IClosedOrderRepository, ClosedOrderRepository>();
builder.Services.AddScoped<ITradingService, TradingService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddSingleton<IPlatformOrderIDGen, PlatformOrderIDGen>(); 
     
builder.Services.AddSingleton<GenericMessageBus<FeatureDataDTO>>();
builder.Services.AddSingleton<OrderManagerService>();  

builder.Services.AddHostedService<FeatureDataProcessor>();     

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost(PlatformConstants.PROCESS_RAW_FEATURE_DATA, async (GenericMessageBus<FeatureDataDTO> featureDataBus, FeatureDataDTO featureData ) =>
      {
        await featureDataBus.PublishAsync(featureData);
        return Results.Ok();
      })
      .WithName("ProcessRawFeatureData")
      .WithOpenApi();

app.Run();
