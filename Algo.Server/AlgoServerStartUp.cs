using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OMS.Core.Common;
using Orleans.Configuration;

using Algo.Server.Services;
using Algo.Trader.Models;
using Algo.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
namespace Algo.Server;

public class AlgoServerStartUp
{
    public IClientBuilder ConfigureClient(IClientBuilder clientBuilder)  
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

        clientBuilder.AddMemoryStreams(PlatformConstants.OrderStreamProvider); 
        

        return clientBuilder;    

    }


    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<AlgoLoaderService1>();
        services.AddSingleton<AlgoOrderQueue>();
        services.AddHostedService<OrderBackgroundService>();                
        return services;
    }

    public IApplicationBuilder Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        return app;
    }


}