using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OMS.Core.Common;
using Orleans.Configuration;


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Strategy.Server.Services;

namespace Strategy.Server;

public class StrategyServerStartUp
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
        services.AddHostedService<BasicService>();
        services.AddHostedService<FadeService>();
        services.AddHostedService<FollowService>();

        services.AddSingleton<StrategyOrderQueue>();
        services.AddHostedService<OrderBackgroundService>();
        return services;
    }

    public IApplicationBuilder Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        return app;
    }


}