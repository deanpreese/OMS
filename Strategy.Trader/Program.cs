using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Strategy.Trader.Services;
using Strategy.Trader.StrategyServices;
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
        services.AddSingleton<IncomingOrderQueue>();
        services.AddHostedService<OrderPubSubBackgroundService>();

        //services.AddHostedService<BasicService>();
        services.AddHostedService<FollowService>();
        //services.AddHostedService<CounterService>();
        services.AddHostedService<FadeService>();
    })
    .Build();

await host.StartAsync();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();