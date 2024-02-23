using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OMS.Core.Common;
using Orleans.Configuration;

using Algo.Server.Services;
using Algo.Trader.Models;
using Algo.Server;

AlgoServerStartUp  algoServerStartUp = new AlgoServerStartUp();

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