
using OMS.Startup;
using OMS.Server.Services;
using OMS.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    OrleansStartupAlgo orleansStartup = new OrleansStartupAlgo();
    siloBuilder = orleansStartup.ConfigureSilo(siloBuilder); 
});

 builder.Services.AddDbContext<OrderManagementDbContext>(options =>
        {
            string conn =  "Host=127.0.0.1;Database=orders;Username=trading;Password=abc";
            options.UseNpgsql(conn);

        },ServiceLifetime.Scoped);


builder.Services.AddSingleton<AlgoOrderQueue>();
builder.Services.AddHostedService<AlgoOrderQueueProcessorService>();
builder.Services.AddHostedService<OrderBackgroundService>();
var app = builder.Build();

app.Run();
