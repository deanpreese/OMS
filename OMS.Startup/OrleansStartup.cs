
using System.Text.Json.Serialization;
using OMS.Data;
using Microsoft.EntityFrameworkCore;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Services.Trading;
using OMS.Services.Data ;
using OMS.Services.Queue;
using OMS.Data.Repositories;
using OMS.Startup;
using OMS.Core.Logging; 
using Orleans.Configuration;
using Orleans.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;


namespace OMS.Startup;

public class OrleansStartup
{
    public ISiloBuilder ConfigureSilo(ISiloBuilder siloBuilder)
    {
        
            siloBuilder
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = "Npgsql";
                    options.ConnectionString = "Host=127.0.0.1;Database=orleans;Username=orleansuser;Password=abc";
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .UseAdoNetReminderService(options =>
                {
                    options.Invariant = "Npgsql";
                    options.ConnectionString = "Host=127.0.0.1;Database=orleans;Username=orleansuser;Password=abc";
                })
                .AddAdoNetGrainStorage("GrainStorage", options =>
                {
                    options.Invariant = "Npgsql";
                    options.ConnectionString = "Host=127.0.0.1;Database=orleans;Username=orleansuser;Password=abc";
                })
                
                .AddMemoryGrainStorage(PlatformConstants.OrderMemoryStreamStore)
                .AddMemoryGrainStorage(PlatformConstants.GrainMemoryStreamStore)                    
                .AddMemoryStreams(PlatformConstants.OrderStreamProvider);

            siloBuilder.ConfigureLogging(logging =>
            {
                logging.AddConsole();
                //logging.AddFilter("Microsoft", LogLevel.Warning);
                //logging.AddFilter("System", LogLevel.Warning);
                //logging.AddFilter("Orleans", LogLevel.Debug);
            });

            siloBuilder.UseDashboard(options => {
                options.Host = "*";
                options.Port = 8080;
                options.HostSelf = true;
                options.CounterUpdateIntervalMs = 5000;
            }
            );

        return siloBuilder;   
    }

}