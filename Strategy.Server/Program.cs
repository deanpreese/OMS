using Microsoft.Extensions.Hosting;
using Strategy.Server;

StrategyServerStartUp  algoServerStartUp = new StrategyServerStartUp();

using IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(client =>
    {
        algoServerStartUp.ConfigureClient(client);
    })
    .UseConsoleLifetime().ConfigureServices(services =>
    {
        algoServerStartUp.ConfigureServices(services);
    })
    .Build();

await host.StartAsync();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();