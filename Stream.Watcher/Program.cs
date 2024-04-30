using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stream.Watcher;

using IHost host = Host.CreateDefaultBuilder(args)
   
    .UseConsoleLifetime().ConfigureServices(services =>
    {
        services.AddHostedService<StreamWatcherService>();
    })
    .Build();

await host.StartAsync();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();

await host.StopAsync();