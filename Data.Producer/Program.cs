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
using Data.Producer;


using IHost host = Host.CreateDefaultBuilder(args)
   
    .UseConsoleLifetime().ConfigureServices(services =>
    {
        services.AddHostedService<PulsarProducerService>();
    })
    .Build();

await host.StartAsync();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();

await host.StopAsync();