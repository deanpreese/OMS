using Pulsar.Watcher;
using DotPulsar;
using DotPulsar.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
   
    .UseConsoleLifetime().ConfigureServices(services =>
    {
        services.AddHostedService<PulsarStrategyOrderWatcherService>();
    })
    .Build();

await host.StartAsync();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();

await host.StopAsync();