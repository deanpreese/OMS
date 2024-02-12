
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Orleans.Streams;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;

using OMS.Core.Common;
using Spectre.Console;
using Orleans.Configuration;
using OMS.Core.Models;
using OMS.Services.Queue;
using OMS.Algo.Queue;

namespace  OMS.Algo;

class ClientProgram
{
    static async Task Main(string[] args)
    {
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
                services.AddSingleton<AlgoOrderQueue>();
                services.AddHostedService<AlgoOrderQueueProcessorService>();
            })
            .Build();

        await host.StartAsync();

        var client = host.Services.GetRequiredService<IClusterClient>();
        var queue = host.Services.GetRequiredService<AlgoOrderQueue>();
        var openOrderStreamProvider = client.GetStreamProvider(PlatformConstants.OrderStreamProvider)
                    .GetStream<OrderInfo>(PlatformConstants.MemoryStreamNamespace, "/orders");


        await Task.WhenAll(

            openOrderStreamProvider.SubscribeAsync(
                async (newOrderUserInfo, token) =>
                {
                    await queue.WriteAsync(newOrderUserInfo);
                })
        );

        Console.WriteLine("Press Enter to terminate...");
        Console.ReadLine();
    }



}
