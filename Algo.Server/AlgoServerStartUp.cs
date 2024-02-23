using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Algo.Server.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using OMS.Core.Logging;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

namespace Algo.Server;

public class AlgoServerStartUp
{
    public IClientBuilder ConfigureClient(HostBuilderContext context, IClientBuilder clientBuilder)  
    {
        clientBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "OrleansBasics";
        });
        clientBuilder.UseAdoNetClustering(options =>
        {
            options.Invariant = "Npgsql";
            options.ConnectionString = "host=10.0.0.147;database=orleans;password=abc;username=orleansuser";
        });

        clientBuilder.AddMemoryStreams("OrderStreamProvider"); // Ensure "OrderStreamProvider" matches with your PlatformConstants.OrderStreamProvider
        

        return clientBuilder;    

    }


    public  IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseOrleansClient(client =>
                {
                    client.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "OrleansBasics";
                    });
                    client.UseAdoNetClustering(options =>
                    {
                        options.Invariant = "Npgsql";
                        options.ConnectionString = "host=10.0.0.147;database=orleans;password=abc;username=orleansuser";
                    });

                    client.AddMemoryStreams("OrderStreamProvider"); // Ensure "OrderStreamProvider" matches with your PlatformConstants.OrderStreamProvider
                })
                .UseConsoleLifetime()
                .ConfigureServices((hostContext, services) =>
                {
                    // Ensure the services like AlgoLoaderService1, AlgoOrderQueue, and OrderBackgroundService are correctly referenced
                    services.AddHostedService<AlgoLoaderService1>(); // Replace with your actual service
                    services.AddSingleton<AlgoOrderQueue>(); // Replace with your actual service
                    services.AddHostedService<OrderBackgroundService>(); // Replace with your actual service

                    // Uncomment and replace these with your actual services if needed
                    //services.AddHostedService<AlgoLoaderService2>();
                    //services.AddHostedService<AlgoLoaderService3>();
                });

    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<AlgoLoaderService1>();
        services.AddSingleton<AlgoOrderQueue>();
        services.AddHostedService<OrderBackgroundService>();                
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