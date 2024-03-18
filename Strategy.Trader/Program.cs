using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OMS.Core.Common;
using Orleans.Configuration;
using Strategy.Trader;
using Strategy.Trader.Services;

using OMS.SharedKernel.Common;


using IHost host = Host.CreateDefaultBuilder(args)
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

        client.AddMemoryStreams(PlatformConstants.OrderStreamProvider);
    })
    .UseConsoleLifetime().ConfigureServices(services =>
    {
        services.AddSingleton<StrategyOrderQueue>();
        services.AddHostedService<OrderBackgroundService>();

        services.AddHostedService<BasicService>();
        //services.AddHostedService<FollowService>();
        //services.AddHostedService<CounterService>();
        //services.AddHostedService<FadeService>();
    })
    .Build();

await host.StartAsync();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();