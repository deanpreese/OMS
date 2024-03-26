using System;
using Npgsql;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Strategy.Server.Services;

using OMS.SharedKernel.Common;
using Strategy.Server;


using IHost host = Host.CreateDefaultBuilder(args)
   
    .UseConsoleLifetime().ConfigureServices(services =>
    {
        services.AddSingleton<IncomingOrderQueue>();
        //services.AddHostedService<OrderPubSubBackgroundService>();
        services.AddHostedService<WatcherService>();

        //services.AddHostedService<BasicService>();
        //services.AddHostedService<FollowService>();ß
        //services.AddHostedService<CounterService>();
        //services.AddHostedService<FadeService>();

        services.AddHostedService<StrategyServiceTest>();
        services.AddHostedService<StrategyServiceTest2>();
        services.AddHostedService<StrategyServiceTest3>();

    })
    .Build();

await host.StartAsync();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();