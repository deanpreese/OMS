<<<<<<<< HEAD:Strategy.Ninja/Program.cs
var builder = WebApplication.CreateBuilder(args);

/*
builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    OrleansStartupAlgo orleansStartup = new OrleansStartupAlgo();
    siloBuilder = orleansStartup.ConfigureSilo(siloBuilder); 
});
*/

 builder.Services.AddDbContext<OrderManagementDbContext>(options =>
        {
            string conn =  "Host=127.0.0.1;Database=orders;Username=trading;Password=abc";
            options.UseNpgsql(conn);

        },ServiceLifetime.Scoped);



builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); 

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDataService, DataService>();

builder.Services.AddSingleton<DataQueue>();
builder.Services.AddHostedService<DataProcessor>();
builder.Services.AddHostedService<NinjaTraderService>();



builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true) // allow any origin
        .AllowCredentials()); // allow credentials

// app.UseHttpsRedirection();

app.UseRouting();
//app.UseAuthorization();
//app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
========
using OMS.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using OMS.NinjaTrader;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Data.Repositories;
using OMS.Infrastructure.Services.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;



var builder = WebApplication.CreateBuilder(args);

/*
builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    OrleansStartupAlgo orleansStartup = new OrleansStartupAlgo();
    siloBuilder = orleansStartup.ConfigureSilo(siloBuilder); 
});
*/

 builder.Services.AddDbContext<OrderManagementDbContext>(options =>
        {
            string conn =  "Host=127.0.0.1;Database=orders;Username=trading;Password=abc";
            options.UseNpgsql(conn);

        },ServiceLifetime.Scoped);



builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); 

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDataService, DataService>();

builder.Services.AddSingleton<DataQueue>();
builder.Services.AddHostedService<DataProcessor>();
builder.Services.AddHostedService<NinjaTraderService>();



builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true) // allow any origin
        .AllowCredentials()); // allow credentials

// app.UseHttpsRedirection();

app.UseRouting();
//app.UseAuthorization();
//app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
>>>>>>>> origin/main:Strategy.Trading/Program.cs
