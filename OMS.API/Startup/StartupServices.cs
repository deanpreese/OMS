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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using OMS.Services.Common;

namespace OMS.API.Startup;

public class StartupServices
{
    public IConfiguration Configuration { get; }
    
    public StartupServices(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        
        services.AddDbContext<OrderManagementDbContext>(options =>
        {
            string conn =  "Host=127.0.0.1;Database=orders;Username=trading;Password=abc";
            options.UseNpgsql(conn);
            options.EnableSensitiveDataLogging();
            options.EnableThreadSafetyChecks();

        },ServiceLifetime.Scoped);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITradingService, TradingService>();
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();


        //services.AddHttpsRedirection(opt => opt.HttpsPort = 44300);

        services.AddSingleton<IPlatformOrderIDGen, PlatformOrderIDGen>();        
        services.AddSingleton<NewOrderChannelService>();
        services.AddSingleton<ClosedOrderChannelService>();
        
        services.AddHostedService<NewOrderProcessorService>();
        services.AddHostedService<ClosedOrderProcessorService>();

        return services;

    }


    public IApplicationBuilder Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        
        return app;


    }
}