using OMS.Application.Common;

using Orleans.Configuration;
using Orleans.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using OMS.SharedKernel.Common;
using OMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OMS.Application.Interfaces;
using OMS.Infrastructure.Services;
using OMS.Infrastructure.Interfaces;
using OMS.Infrastructure.Services.Common;
using OMS.Infrastructure.Queue;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    siloBuilder
        .UseAdoNetClustering(options =>
        {
            options.Invariant = "Npgsql";
            //options.ConnectionString = "Host=127.0.0.1;Database=orleans;Username=orleansuser;Password=abc";
            options.ConnectionString = "Server=127.0.0.1;Database=orleans;Username=orleansuser;Password=abc";
        })
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "OrleansBasics";
        })
        .UseAdoNetReminderService(options =>
        {
            options.Invariant = "Npgsql";
            //options.ConnectionString = "Host=127.0.0.1;Database=orleans;Username=orleansuser;Password=abc";
            options.ConnectionString = "Server=127.0.0.1;Database=orleans;Username=orleansuser;Password=abc";
        })
        .AddAdoNetGrainStorage("GrainStorage", options =>
        {
            options.Invariant = "Npgsql";
            //options.ConnectionString = "Host=127.0.0.1;Database=orleans;Username=orleansuser;Password=abc";
            options.ConnectionString = "Server=127.0.0.1;Database=orleans;Username=orleansuser;Password=abc";
        })
        
        .AddMemoryGrainStorage(PlatformConstants.OrderMemoryStreamStore)
        .AddMemoryGrainStorage(PlatformConstants.GrainMemoryStreamStore) 

        .AddMemoryStreams(PlatformConstants.OrderStreamProvider);

    siloBuilder.ConfigureLogging(logging =>
    {
        logging.AddConsole();
        //logging.AddFilter("Microsoft", LogLevel.Warning);
        //logging.AddFilter("System", LogLevel.Warning);
        //logging.AddFilter("Orleans", LogLevel.Debug);
    });

    siloBuilder.UseDashboard(options => {
        options.Host = "*";
        options.Port = 8080;
        options.HostSelf = true;
        options.CounterUpdateIntervalMs = 5000;
    }
    );


});


 builder.Services.AddDbContext<OrderManagementDbContext>(options =>
{
    //string conn =  "Host=127.0.0.1;Database=orders;Username=trading;Password=abc";
    string conn =  "Server=127.0.0.1;Database=orders;Username=trading;Password=abc";
    options.UseNpgsql(conn);
    //options.EnableSensitiveDataLogging();
    options.EnableThreadSafetyChecks();

},ServiceLifetime.Scoped);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILiveOrderRepository, LiveOrderRepository>();
builder.Services.AddScoped<IClosedOrderRepository, ClosedOrderRepository>();
builder.Services.AddScoped<ITradingService, TradingService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();


//services.AddHttpsRedirection(opt => opt.HttpsPort = 44300);

builder.Services.AddSingleton<IPlatformOrderIDGen, PlatformOrderIDGen>();        
builder.Services.AddSingleton<NewOrderChannelService>();
builder.Services.AddSingleton<ClosedOrderChannelService>();

builder.Services.AddHostedService<NewOrderProcessorService>();
builder.Services.AddHostedService<ClosedOrderProcessorService>();



var app = builder.Build();
await app.RunAsync();
//app.Run();
