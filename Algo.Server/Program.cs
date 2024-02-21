using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OMS.Core.Common;
using Orleans.Configuration;

using Algo.Algorithms.Services;
using Algo.Algorithms.Models;


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
        services.AddHostedService<AlgoLoaderService1>();
        services.AddSingleton<AlgoOrderQueue>();
        services.AddHostedService<OrderBackgroundService>();                
       
        //services.AddHostedService<AlgoLoaderService2>();
        //services.AddHostedService<AlgoLoaderService3>();

        
        
    })
    .Build();

await host.StartAsync();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();