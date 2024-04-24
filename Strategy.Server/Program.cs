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
using Strategy.Server.StrategyServices;


using IHost host = Host.CreateDefaultBuilder(args)
   
    .UseConsoleLifetime().ConfigureServices(services =>
    {
        services.AddSingleton<ModelOrderMessageBus>();                
        
        services.AddHostedService<WatcherService>();
        //services.AddHostedService<PulsarModelLogWatcher>();

        //services.AddHostedService<NGZero>(); //FollowWinners
        services.AddHostedService<StrategyService>();  // OC
        //services.AddHostedService<NGTwo>();   //Follow
        //services.AddHostedService<NGThree>();   //Fade
        

    })
    .Build();

await host.StartAsync();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();

await host.StopAsync();