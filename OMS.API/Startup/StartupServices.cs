using Microsoft.EntityFrameworkCore;
using OMS.Core.Interfaces;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Data.Repositories;
using OMS.Infrastructure.Services.Common;
using OMS.Infrastructure.Services.Data;
using OMS.Infrastructure.Services.Queue;
using OMS.Infrastructure.Services.Trading;
using OMS.Infrastructure.Interfaces;

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
            //string conn =  "Host=127.0.0.1;Database=orders;Username=trading;Password=abc";
            string conn =  "Server=127.0.0.1;Database=orders;Username=trading;Password=abc";
            options.UseNpgsql(conn);
            //options.EnableSensitiveDataLogging();
            options.EnableThreadSafetyChecks();

        },ServiceLifetime.Scoped);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ILiveOrderRepository, LiveOrderRepository>();
        services.AddScoped<IClosedOrderRepository, ClosedOrderRepository>();
        services.AddScoped<ITradingService, TradingService>();
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();


        //services.AddHttpsRedirection(opt => opt.HttpsPort = 44300);

        services.AddSingleton<IPlatformOrderIDGen, PlatformOrderIDGen>();        
        services.AddSingleton<NewOrderChannelService>();
        services.AddSingleton<ClosedOrderChannelService>();
        services.AddSingleton<FeatureDataDataQueue>();
        
        services.AddHostedService<NewOrderProcessorService>();
        services.AddHostedService<ClosedOrderProcessorService>();
        services.AddHostedService<FeatureDataProcessor>();

        return services;

    }


    public IApplicationBuilder Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        
        return app;


    }
}