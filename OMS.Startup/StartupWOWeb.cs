using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Text.Json.Serialization;
using OMS.Data;
using Microsoft.EntityFrameworkCore;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Services.Trading;
using OMS.Services.Data;
using OMS.Services.Queue;
using OMS.Data.Repositories;
using OMS.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using OMS.Services.Common;

namespace OMS.Startup;

public class StartupWOWeb
{
    public IConfiguration Configuration { get; }
    
    public StartupWOWeb(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        
        services.AddDbContext<OrderManagementDbContext>(options =>
        {
            string conn =  "Host=127.0.0.1;Database=orders;Username=trading;Password=abc";
            options.UseNpgsql(conn);

        },ServiceLifetime.Scoped);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITradingService, TradingService>();
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();


        //services.AddHttpsRedirection(opt => opt.HttpsPort = 44300);

        services.AddSingleton<IPlatformOrderIDGen, PlatformOrderIDGen>();        
        services.AddSingleton<NewOrderChannelService>();
        services.AddSingleton<ClosedOrderChannelService>();
        
        services.AddHostedService<NewOrderProcessorService>();
        services.AddHostedService<ClosedOrderProcessorService>();

        return services;

    }

    public void AddSpectreLogging(IServiceCollection services)
    {
        services.AddLogging(logging =>
        {  
            logging.ClearProviders();
            logging.AddSpectreConsole(); 
            //logging.AddConsole();   
        });
    }

    public IApplicationBuilder Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        
        return app;


    }
}